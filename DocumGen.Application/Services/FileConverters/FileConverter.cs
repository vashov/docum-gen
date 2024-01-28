using DocumGen.Application.Contracts.FileStorages;
using DocumGen.Application.Services.Configuration;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Media;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumGen.Application.Services.FileConverters
{
    public class FileConverter : IFileConverter
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileConverterConfiguration _converterConfiguration;

        public FileConverter(
            IFileStorage fileStorage,
            IFileConverterConfiguration pdfConfiguration) 
        {
            _fileStorage = fileStorage;
            _converterConfiguration = pdfConfiguration;
        }

        public async Task<string> PrepareBrowser(CancellationToken cancellation)
        {
            BrowserFetcherOptions browserFetcherOptions = BuildBrowserFetchOptions();
            using var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            var installedBrowser = GetInstalledBrowser(browserFetcher);
            if (installedBrowser == null)
            {
                await Task.Run(async () => await browserFetcher.DownloadAsync(), cancellation);
                installedBrowser = GetInstalledBrowser(browserFetcher);
            }

            return installedBrowser.GetExecutablePath();
        }

        public async Task<bool> ConvertHtmlToPdf(string fileNameSource, string fileNameResult, CancellationToken cancellation)
        {
            bool isExists = await _fileStorage.Exists(fileNameSource);
            if (!isExists)
                return false;

            string browserExecutablePath = await PrepareBrowser(cancellation);
            LaunchOptions launchOptions = BuildLaunchOptions(browserExecutablePath);
            NavigationOptions navigationOptions = BuildNavigationOptions();
            //BrowserFetcherOptions browserFetcherOptions = BuildBrowserFetchOptions();

            //using var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            //var installed = browserFetcher.GetInstalledBrowsers();
            //installed.GetExecutablePath();

            using Stream stream = _fileStorage.OpenRead(fileNameSource);
            using IBrowser browser = await Puppeteer.LaunchAsync(launchOptions);
            using IPage page = await browser.NewPageAsync();
            using StreamReader streamReader = new StreamReader(stream);

            string line = await streamReader.ReadToEndAsync();
            await page.SetContentAsync(line, navigationOptions);

            PdfOptions pdfOptions = BuildPdfOptions();
            Stream pdfStream = await page.PdfStreamAsync(pdfOptions);

            bool created = await _fileStorage.CreateWrite(fileNameResult, pdfStream);
            return created;
        }

        private BrowserFetcherOptions BuildBrowserFetchOptions()
        {
            return new BrowserFetcherOptions()
            {
                Path = _converterConfiguration.BrowserDownloadPath
            };
        }

        private NavigationOptions BuildNavigationOptions()
        {
            return new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                Timeout = _converterConfiguration.NavigationTimeout
            };
        }

        private LaunchOptions BuildLaunchOptions(string browserExecutablePath)
        {
            return new LaunchOptions
            {
                Headless = !_converterConfiguration.ShowBrowser,
                ExecutablePath = browserExecutablePath
            };
        }

        private PdfOptions BuildPdfOptions()
        {
            var pdfOptions = new PdfOptions
            {
                Format = new PaperFormat(_converterConfiguration.Width, _converterConfiguration.Height),
                MarginOptions = new MarginOptions
                {
                    Left = GetFormattedMargin(_converterConfiguration.MarginLeft),
                    Right = GetFormattedMargin(_converterConfiguration.MarginRight),
                    Top = GetFormattedMargin(_converterConfiguration.MarginTop),
                    Bottom = GetFormattedMargin(_converterConfiguration.MarginBottom)
                },
                Scale = _converterConfiguration.Scale,
                PrintBackground = _converterConfiguration.PrintBackground,
            };
            return pdfOptions;
        }

        private InstalledBrowser GetInstalledBrowser(BrowserFetcher browserFetcher)
        {
            var installedBrowser = browserFetcher.GetInstalledBrowsers().FirstOrDefault();
            return installedBrowser;
        }

        private string GetFormattedMargin(int margin) => $"{margin}px";
    }
}
