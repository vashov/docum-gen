using System.Text.Json;

namespace DocumGen.MessageBus.RabbitMq
{
    internal static class JsonSerializerHelper
    {
        public static JsonSerializerOptions GetDefault()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }
    }
}
