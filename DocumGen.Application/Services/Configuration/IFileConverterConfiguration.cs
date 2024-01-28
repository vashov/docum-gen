namespace DocumGen.Application.Services.Configuration
{
    public interface IFileConverterConfiguration
    {
        decimal Width { get; }
        decimal Height { get; }
        decimal Scale { get; }

        int MarginLeft { get; }
        int MarginRight { get; }
        int MarginTop { get; }
        int MarginBottom { get; }

        bool PrintBackground { get; }
        bool ShowBrowser { get; }

        int NavigationTimeout { get; }
        string BrowserDownloadPath { get; }
    }
}
