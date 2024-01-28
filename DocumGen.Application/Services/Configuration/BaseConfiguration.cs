using DocumGen.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumGen.Application.Services.Configuration
{
    public abstract class BaseConfiguration
    {
        protected IConfiguration Configuration { get; }

        protected abstract string ConfigurationName { get; }
        protected abstract void Init(IConfiguration configuration, List<string> errors);

        /// <summary>
        /// Invoke validation at end and throw ConfigurationException if any errors exist.
        /// </summary>
        /// <exception cref="ConfigurationException" />
        public BaseConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;

            List<string> errors = new List<string>();
            Init(configuration, errors);
            Validate(errors);
        }

        private void Validate(List<string> errors)
        {
            if (errors.Any())
                throw new ConfigurationException(ConfigurationName, errors);
        }

        protected T GetValue<T>(string key, string defaultValue, List<string> errors, Func<T, string> getValidationError = null)
        {
            string fullKey = $"{ConfigurationName}:{key}";

            string valueString = Configuration[fullKey];
            if (string.IsNullOrEmpty(valueString) && defaultValue != null)
            {
                valueString = defaultValue;
            }

            T value = default;

            if (valueString == null)
            {
                errors.Add(FormatErrorMessage(fullKey, "is required"));
                return value;
            }

            try
            {
                value = (T)Convert.ChangeType(valueString, typeof(T));
            }
            catch (Exception ex)
            {
                errors.Add(FormatErrorMessage(fullKey, ex.Message));
                return value;
            }

            if (getValidationError != null)
            {
                string errorMsg = getValidationError(value);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    errors.Add(FormatErrorMessage(fullKey, errorMsg));
                }
            }
            return value;
        }

        private static string FormatErrorMessage(string fullKey, string message) => $"{fullKey}: {message}";
    }
}
