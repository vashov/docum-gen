using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumGen.Application.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message, List<string> errors) : base(GetDescription(message, errors))
        {
        }

        private static string GetDescription(string message, List<string> errors)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(message);
            if (errors.Any())
            {
                builder.Append(": ");
                builder.AppendJoin(';', errors);
            }

            return builder.ToString();
        }
    }
}
