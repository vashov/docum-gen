using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DocumGen.Application.Services.Configuration
{
    public class FileConverterConfiguration : BaseConfiguration, IFileConverterConfiguration
    {
        public decimal Width { get; private set; }
        public decimal Height { get; private set; }
        public decimal Scale { get; private set; }

        public int MarginLeft { get; private set; }
        public int MarginRight { get; private set; }
        public int MarginTop { get; private set; }
        public int MarginBottom { get; private set; }

        public bool PrintBackground { get; private set; }
        public bool ShowBrowser { get; private set; }
        public int NavigationTimeout { get; private set; }
        public string BrowserDownloadPath { get; private set; }

        public FileConverterConfiguration(IConfiguration configuration) : base(configuration) { }

        protected override string ConfigurationName => "FileConverter";

        protected override void Init(IConfiguration configuration, List<string> errors)
        {
            Func<int, string> intNotLessThenZero = (value) => (value < 0) ? $"should be no less then {0}" : null;
            Func<decimal, string> decimalNotLessThenZero = (value) => (value < 0) ? $"should be no less then {0}" : null;

            Width = GetValue<decimal>("Width", defaultValue: "8.27", errors, getValidationError: decimalNotLessThenZero);
            Height = GetValue<decimal>("Height", defaultValue: "11.7", errors, getValidationError: decimalNotLessThenZero);

            Scale = GetValue<decimal>("Scale", defaultValue: "1", errors, 
                getValidationError: (value) =>
                {
                    const decimal MinScale = 0.2m;
                    const decimal MaxScale = 2m;
                    if (value < MinScale || value > MaxScale)
                        return $"should be no less then {MinScale} and no more then {MaxScale}";
                    return null;
                });

            MarginLeft = GetValue<int>("MarginLeft", defaultValue: "20", errors, getValidationError: intNotLessThenZero);
            MarginRight = GetValue<int>("MarginRight", defaultValue: "20", errors, getValidationError: intNotLessThenZero);
            MarginTop = GetValue<int>("MarginTop", defaultValue: "20", errors, getValidationError: intNotLessThenZero);
            MarginBottom = GetValue<int>("MarginBottom", defaultValue: "20", errors, getValidationError: intNotLessThenZero);

            PrintBackground = GetValue<bool>("PrintBackground", defaultValue: "false", errors, getValidationError: null);
            ShowBrowser = GetValue<bool>("ShowBrowser", defaultValue: "false", errors, getValidationError: null);
            NavigationTimeout = GetValue<int>("NavigationTimeout", defaultValue: "0", errors, getValidationError: intNotLessThenZero);

            BrowserDownloadPath = GetValue<string>("BrowserDownloadPath", defaultValue: "", errors, getValidationError: null);
        }
    }
}
