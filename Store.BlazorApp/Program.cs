using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Store.BlazorApp.Services;
using Store.BlazorApp.Services.Interfaces;

namespace Store.BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient<IBookService, BookService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7056");
            });

            await builder.Build().RunAsync();
        }
    }
}
