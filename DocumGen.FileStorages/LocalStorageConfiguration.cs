using DocumGen.Application.Services.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DocumGen.FileStorages
{
    public class LocalStorageConfiguration : BaseConfiguration
    {
        public string Path { get; private set; }
        protected override string ConfigurationName => "LocalStorage";

        public LocalStorageConfiguration(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void Init(IConfiguration configuration, List<string> errors)
        {
            Path = GetValue<string>("Path", defaultValue: null, errors, getValidationError: null);
        }
    }
}
