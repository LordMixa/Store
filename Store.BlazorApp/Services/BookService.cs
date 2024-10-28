using Store.BlazorApp.Models;
using Store.BlazorApp.Services.Interfaces;
using System.Net.Http.Json;

namespace Store.BlazorApp.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BookModel>> GetBooksAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<BookModel>>("/api/books");

            return response;
        }
    }
}
