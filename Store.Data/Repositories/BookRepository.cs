using Microsoft.Data.SqlClient;
using Store.Data.Dtos;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;
using System.Transactions;

namespace Store.Data.Repositories
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

            string query = @"
                    SELECT 
                        b.Id,
                        b.Title,
                        b.Price,
                        b.DateOfPublication,
                        b.Description,
                        a.Id AS AuthorId,
                        a.FirstName,
                        a.LastName,
                        a.Biography,
                        c.Id AS CategoryId,
                        c.Name
                    FROM 
                        Books b
                    LEFT JOIN 
                        BookAuthor ba ON b.Id = ba.BookId
                    LEFT JOIN 
                        Authors a ON ba.AuthorId = a.Id
                    LEFT JOIN 
                        BookCategory bc ON b.Id = bc.BookId
                    LEFT JOIN 
                        Categories c ON bc.CategoryId = c.Id
                    WHERE 
                        b.Id = @Id";

            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = query;

                command.Parameters.Add(new SqlParameter($"@{nameof(Book.Id)}", id));

                await sqlConnection.OpenAsync();

                var book = await ReadBookSql(command);

                return book;
            }
        }

        public async Task<IEnumerable<BookDto>?> GetAsync()
        {
            if (_dbConnection is not SqlConnection sqlConnection)
            {
                return null;
            }

            string query = @"
                    SELECT 
                        b.Id,
                        b.Title,
                        b.Price,
                        b.DateOfPublication,
                        b.Description,
                        a.Id AS AuthorId,
                        a.FirstName,
                        a.LastName,
                        a.Biography,
                        c.Id AS CategoryId,
                        c.Name
                    FROM 
                        Books b
                    LEFT JOIN 
                        BookAuthor ba ON b.Id = ba.BookId
                    LEFT JOIN 
                        Authors a ON ba.AuthorId = a.Id
                    LEFT JOIN 
                        BookCategory bc ON b.Id = bc.BookId
                    LEFT JOIN 
                        Categories c ON bc.CategoryId = c.Id";

            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = query;

                await sqlConnection.OpenAsync();

                var books = await ReadBooksSql(command);

                return books;
            }
        }

        public async Task<int> CreateAsync(Book book)
        {
            int bookId = default;

            if (_dbConnection is not SqlConnection sqlConnection)
            {
                return default;
            }

            var query = new StringBuilder(@"
                                INSERT INTO Books (Title, Price, DateOfPublication, Description) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Title, @Price, @DateOfPublication, @Description);");

            await sqlConnection.OpenAsync();

            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Title)}", book.Title));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Price)}", book.Price));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.DateOfPublication)}", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Description)}", book.Description ?? (object)DBNull.Value));

                        command.CommandText = query.ToString();

                        bookId = (int)await command.ExecuteScalarAsync();
                    }

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        string newQuery = string.Empty;

                        if (book.Authors.Any())
                        {
                            book.Authors = await GetAuthors(sqlConnection, book.Authors, transaction);

                            newQuery = AddAuthors(bookId, book.Authors).Item1;
                            var parametrs = AddAuthors(bookId, book.Authors).Item2;

                            foreach (var param in parametrs)
                                command.Parameters.Add(param);
                        }
                        if (book.Categories.Any())
                        {
                            book.Categories = await GetCategories(sqlConnection, book.Categories, transaction);

                            newQuery += AddCategories(bookId, book.Categories).Item1;
                            var parametrs = AddCategories(bookId, book.Categories).Item2;

                            foreach (var param in parametrs)
                                command.Parameters.Add(param);
                        }

                        command.CommandText = newQuery;

                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return bookId;
        }

        public async Task<bool> UpdateAsync(Book book)
        {
            if (_dbConnection is not SqlConnection sqlConnection)
            {
                return false;
            }

            var query = new StringBuilder(@"
                                UPDATE Books
                                SET Title = @Title,
                                    Price = @Price,
                                    DateOfPublication = @DateOfPublication,
                                    Description = @Description
                                WHERE Id = @Id");

            await sqlConnection.OpenAsync();

            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Title)}", book.Title));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Price)}", book.Price));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.DateOfPublication)}", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Description)}", book.Description ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Id)}", book.Id));

                        command.CommandText = query.ToString();

                        await command.ExecuteNonQueryAsync();

                    }

                    await DeleteBookCategories(book.Id, sqlConnection, transaction);
                    await DeleteBookAuthors(book.Id, sqlConnection, transaction);

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        string newQuery = string.Empty;

                        if (book.Authors.Any())
                        {
                            book.Authors = await GetAuthors(sqlConnection, book.Authors, transaction);

                            newQuery = AddAuthors(book.Id, book.Authors).Item1;
                            var parametrs = AddAuthors(book.Id, book.Authors).Item2;

                            foreach (var param in parametrs)
                                command.Parameters.Add(param);
                        }
                        if (book.Categories.Any())
                        {
                            book.Categories = await GetCategories(sqlConnection, book.Categories, transaction);

                            newQuery += AddCategories(book.Id, book.Categories).Item1;
                            var parametrs = AddCategories(book.Id, book.Categories).Item2;

                            foreach (var param in parametrs)
                                command.Parameters.Add(param);
                        }

                        command.CommandText = newQuery;

                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();

                    return false;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_dbConnection is not SqlConnection sqlConnection)
            {
                return false;
            }

            string query = @"
                             DELETE FROM Books
                             WHERE Id = @Id";

            await sqlConnection.OpenAsync();

            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = query;

                command.Parameters.Add(new SqlParameter($"@{nameof(Book.Id)}", id));

                var affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }

        private async Task<Book?> ReadBookSql(SqlCommand command)
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                var book = new Book();
                var authors = new List<Author>();
                var categories = new List<Category>();

                while (await reader.ReadAsync())
                {
                    if (book.Id == 0)
                    {
                        book = ReadBook(reader);
                    }

                    if (!reader.IsDBNull("AuthorId"))
                    {
                        var authorId = reader.GetInt32("AuthorId");
                        if (!authors.Any(a => a.Id == authorId))
                        {
                            var author = new Author
                            {
                                Id = reader.GetInt32("AuthorId"),
                                FirstName = reader.GetString(nameof(Author.FirstName)),
                                LastName = reader.GetString(nameof(Author.LastName)),
                                Biography = reader.GetString(nameof(Author.Biography)),
                            };
                            authors.Add(author);
                        }
                    }
                    if (!reader.IsDBNull("CategoryId"))
                    {
                        var categoryId = reader.GetInt32("CategoryId");
                        if (!categories.Any(a => a.Id == categoryId))
                        {
                            var category = new Category
                            {
                                Id = reader.GetInt32("CategoryId"),
                                Name = reader.GetString(nameof(Category.Name))
                            };
                            categories.Add(category);
                        }
                    }
                }
                book.Authors = authors;
                book.Categories = categories;

                return book;
            }
        }

        private async Task<List<BookDto>> ReadBooksSql(SqlCommand command)
        {
            var bookDtos = new List<BookDto>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var bookDto = new BookDto
                    {
                        Id = reader.GetInt32(nameof(BookDto.Id)),
                        Title = reader.GetString(nameof(BookDto.Title)),
                        DateOfPublication = DateOnly.FromDateTime(reader.GetDateTime(nameof(BookDto.DateOfPublication))),
                        Price = reader.GetDecimal(nameof(BookDto.Price)),
                        Description = reader.GetString(nameof(BookDto.Description)),

                        AuthorId = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.AuthorId))) ? 0 : reader.GetInt32(nameof(BookDto.AuthorId)),
                        FirstName = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.FirstName))) ? string.Empty : reader.GetString(nameof(BookDto.FirstName)),
                        LastName = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.LastName))) ? string.Empty : reader.GetString(nameof(BookDto.LastName)),
                        Biography = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.Biography))) ? string.Empty : reader.GetString(nameof(BookDto.Biography)),

                        CategoryId = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.CategoryId))) ? 0 : reader.GetInt32(nameof(BookDto.CategoryId)),
                        Name = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.Name))) ? string.Empty : reader.GetString(nameof(BookDto.Name))
                    };

                    bookDtos.Add(bookDto);
                }
            }

            return bookDtos;
        }

        private Book ReadBook(SqlDataReader reader)
        {
            var book = new Book();

            book.Id = reader.GetInt32(nameof(Book.Id));
            book.Title = reader.GetString(nameof(Book.Title));
            book.Price = reader.GetDecimal(nameof(Book.Price));
            book.DateOfPublication = DateOnly.FromDateTime(reader.GetDateTime(nameof(Book.DateOfPublication)));
            book.Description = reader.GetString(nameof(Book.Description));

            return book;
        }

        private (string, IEnumerable<SqlParameter>) AddAuthors(int bookId, IEnumerable<Author> authors)
        {
            var query = new StringBuilder();
            var sqlCommands = new List<SqlParameter>
            {
                new SqlParameter("@BookId", bookId)
            };

            foreach (var author in authors)
            {
                sqlCommands.Add(new SqlParameter($"@AuthorId_{author.Id}", author.Id));

                query.Append($@"
                    INSERT INTO BookAuthor (BookId, AuthorId) 
                    VALUES (@BookId, @AuthorId_{author.Id});");
            }

            return (query.ToString(), sqlCommands);
        }

        private (string, IEnumerable<SqlParameter>) AddCategories(int bookId, IEnumerable<Category> categories)
        {
            var query = new StringBuilder();
            var sqlCommands = new List<SqlParameter>();

            foreach (var category in categories)
            {
                sqlCommands.Add(new SqlParameter($"@CategoryId_{category.Id}", category.Id));

                query.Append($@"
                    INSERT INTO BookCategory (BookId, CategoryId) 
                    VALUES (@BookId, @CategoryId_{category.Id});");
            }

            return (query.ToString(), sqlCommands);
        }

        private async Task<IEnumerable<Author>> GetAuthors(SqlConnection sqlConnection, IEnumerable<Author> authors, SqlTransaction transaction)
        {
            string authorsQuery = @"
                SELECT 
                    Id
                FROM 
                    Authors ";

            using (var command = sqlConnection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText = authorsQuery;

                var authorsDB = new List<Author>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var author = new Author
                        {
                            Id = reader.GetInt32(nameof(Author.Id)),
                        };
                        authorsDB.Add(author);
                    }
                }
                authorsDB.RemoveAll(author => !authors.Any(a => a.Id == author.Id));

                return authorsDB;
            }
        }

        private async Task<IEnumerable<Category>> GetCategories(SqlConnection sqlConnection, IEnumerable<Category> categories, SqlTransaction transaction)
        {
            string categoriesQuery = @"
                SELECT 
                    Id
                FROM 
                    Categories ";

            using (var command = sqlConnection.CreateCommand())
            {
                command.Transaction = transaction;

                command.CommandText = categoriesQuery;

                var categoriesDB = new List<Category>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var category = new Category
                        {
                            Id = reader.GetInt32(nameof(Category.Id)),
                        };
                        categoriesDB.Add(category);
                    }
                }
                categoriesDB.RemoveAll(category => !categories.Any(a => a.Id == category.Id));

                return categoriesDB;
            }
        }

        private async Task DeleteBookCategories(int bookId,SqlConnection sqlConnection, SqlTransaction transaction)
        {
            string deleteQuery = @"
                DELETE FROM BookCategory 
                WHERE BookId = @BookId";

            using (var command = sqlConnection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = deleteQuery;
                command.Parameters.AddWithValue("@BookId", bookId);

                await command.ExecuteNonQueryAsync();
            }          
        }

        private async Task DeleteBookAuthors(int bookId, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            string deleteQuery = @"
                DELETE FROM BookAuthor 
                WHERE BookId = @BookId";

            using (var command = sqlConnection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = deleteQuery;
                command.Parameters.AddWithValue("@BookId", bookId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
