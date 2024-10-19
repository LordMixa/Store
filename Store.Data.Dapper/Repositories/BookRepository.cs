using Dapper;
using Microsoft.Data.SqlClient;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Entities;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnection _dbConnection;

        public BookRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Book?> GetAsync(int id)
        {
            if (_dbConnection is not SqlConnection sqlConnection)
            {
                return null;
            }

            using (sqlConnection)
            {
                var parameters = new { @Id = id };

                var books = await sqlConnection.QueryAsync<Book, Author, Category, Book>(
                "Procedure_GetBookById",
                (book, author, category) =>
                {
                    if (author != null && !book.Authors.Any(a => a.AuthorId == author.AuthorId))
                    {
                        book.Authors.Add(author);
                    }
                    if (category != null && !book.Categories.Any(c => c.CategoryId == category.CategoryId))
                    {
                        book.Categories.Add(category);
                    }

                    return book;
                },
                parameters,
                splitOn: "Id,AuthorId,CategoryId",
                commandType: CommandType.StoredProcedure
                );

                var combinedBooks = books
                .GroupBy(book => book.Id)
                .Select(group =>
                {
                    var combinedBook = group.First(); 

                    foreach (var book in group)
                    {
                        foreach (var author in book.Authors)
                        {
                            if (!combinedBook.Authors.Any(a => a.AuthorId == author.AuthorId))
                            {
                                combinedBook.Authors.Add(author);
                            }
                        }
                        foreach (var category in book.Categories)
                        {
                            if (!combinedBook.Categories.Any(c => c.CategoryId == category.CategoryId))
                            {
                                combinedBook.Categories.Add(category);
                            }
                        }
                    }
                    return combinedBook;
                });

                return combinedBooks.FirstOrDefault();
            }
        }
    }
}
