using DocumGen.Application.Contracts.MessageBus;
using DocumGen.Application.Services.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DocumGen.MessageBus.RabbitMq
{
    public class RabbitConfiguration : BaseConfiguration, IMessageBusConfiguration
    {
        public string HostName { get; private set; }
        public int Port { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public string QueueFileOrderNew { get; private set; }
        public string QueueFileOrderProcessed { get; private set; }

        public RabbitConfiguration(IConfiguration configuration) : base(configuration) { }

        protected override string ConfigurationName => "RabbitMq";

        protected override void Init(IConfiguration configuration, List<string> errors)
        {
            HostName = GetValue<string>("HostName", defaultValue: null, errors, getValidationError: null);
            Port = GetValue<int>("Port", defaultValue: null, errors, getValidationError: null);
            UserName = GetValue<string>("UserName", defaultValue: null, errors, getValidationError: null);
            Password = GetValue<string>("Password", defaultValue: null, errors, getValidationError: null);
            HostName = GetValue<string>("HostName", defaultValue: null, errors, getValidationError: null);

            QueueFileOrderNew = GetValue<string>("QueueFileOrderNew", defaultValue: null, errors, getValidationError: null);
            QueueFileOrderProcessed = GetValue<string>("QueueFileOrderProcessed", defaultValue: null, errors, getValidationError: null);
        }
    }
}
