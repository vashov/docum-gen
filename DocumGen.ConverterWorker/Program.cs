using Microsoft.AspNetCore.Builder;

namespace DocumGen.ConverterWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAppServices();

            var app = builder.Build();
            app.Run();
        }
    }
}