using Microsoft.Data.SqlClient;
using Store.Data.Dtos;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;
using System.Data;
using System.Text;

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

                command.Parameters.Add(new SqlParameter($"@{nameof(id)}", id));

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

                        var bookParameters = GetBookParameters(book);

                        foreach (var param in bookParameters)
                            command.Parameters.Add(param);

                        command.CommandText = query.ToString();

                        bookId = (int)await command.ExecuteScalarAsync();

                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Id)}", bookId));

                        book.Id = bookId;

                        await AddBookEntities(book, command);
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
                                WHERE Id = @Id;");

            await sqlConnection.OpenAsync();

            using (var transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        var bookParameters = GetBookParameters(book);

                        foreach (var param in bookParameters)
                            command.Parameters.Add(param);

                        command.Parameters.Add(new SqlParameter($"@{nameof(Book.Id)}", book.Id));

                        var deleteBookQuery = DeleteBookAuthors();
                        query.Append(deleteBookQuery);

                        var deleteCategoryQuery = DeleteBookCategories();
                        query.Append(deleteCategoryQuery);

                        command.CommandText = query.ToString();

                        await command.ExecuteNonQueryAsync();

                        await AddBookEntities(book, command);
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

                while (await reader.ReadAsync())
                {
                    if (book.Id == 0)
                    {
                        book = ReadBook(reader);
                    }

                    if (!reader.IsDBNull("AuthorId"))
                    {
                        var authorId = reader.GetInt32("AuthorId");
                        if (!book.Authors.Any(a => a.Id == authorId))
                        {
                            var author = GetAuthor(reader, authorId);
                            book.Authors.Add(author);
                        }
                    }
                    if (!reader.IsDBNull("CategoryId"))
                    {
                        var categoryId = reader.GetInt32("CategoryId");
                        if (!book.Categories.Any(a => a.Id == categoryId))
                        {
                            var category = GetCategory(reader, categoryId);
                            book.Categories.Add(category);
                        }
                    }
                }

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

        private async Task<(string, IEnumerable<SqlParameter>)> AddAuthors(int bookId, IEnumerable<Author> authors, SqlCommand command)
        {
            var updatedAuthors = await GetExistedAuthors(command, authors);

            var query = new StringBuilder();
            var sqlCommands = new List<SqlParameter>
            {
                new SqlParameter("@BookId", bookId)
            };

            foreach (var author in updatedAuthors)
            {
                sqlCommands.Add(new SqlParameter($"@AuthorId_{author.Id}", author.Id));

                query.Append($@"
                    INSERT INTO BookAuthor (BookId, AuthorId) 
                    VALUES (@Id, @AuthorId_{author.Id});");
            }

            return (query.ToString(), sqlCommands);
        }

        private async Task<(string, IEnumerable<SqlParameter>)> AddCategories(int bookId, IEnumerable<Category> categories, SqlCommand command)
        {
            IEnumerable<Category> updatedCategories = await GetExistedCategories(command, categories);

            var query = new StringBuilder();
            var sqlCommands = new List<SqlParameter>();

            foreach (var category in updatedCategories)
            {
                sqlCommands.Add(new SqlParameter($"@CategoryId_{category.Id}", category.Id));

                query.Append($@"
                    INSERT INTO BookCategory (BookId, CategoryId) 
                    VALUES (@Id, @CategoryId_{category.Id});");
            }

            return (query.ToString(), sqlCommands);
        }

        private async Task<List<int>> GetAuthorIds(SqlCommand command, IEnumerable<Author> authors)
        {
            string authorsQuery = @"
                SELECT 
                    Id
                FROM 
                    Authors ";

            command.CommandText = authorsQuery;

            var authorsDB = new List<int>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int Id = reader.GetInt32(nameof(Author.Id));

                    authorsDB.Add(Id);
                }
            }

            return authorsDB;
        }

        private async Task<List<int>> GetCategoryIds(SqlCommand command, IEnumerable<Category> categories)
        {
            string categoriesQuery = @"
                SELECT 
                    Id
                FROM 
                    Categories ";

            command.CommandText = categoriesQuery;

            var categoriesDB = new List<int>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int Id = reader.GetInt32(nameof(Category.Id));

                    categoriesDB.Add(Id);
                }
            }

            return categoriesDB;
        }

        private async Task<IEnumerable<Author>> GetExistedAuthors(SqlCommand command, IEnumerable<Author> authors)
        {
            var authorsDB = await GetAuthorIds(command, authors);
            var existingAuthors = authors.Where(author => authorsDB.Contains(author.Id)).ToList();

            return existingAuthors;
        }

        private async Task<IEnumerable<Category>> GetExistedCategories(SqlCommand command, IEnumerable<Category> categories)
        {
            var categoriesDB = await GetCategoryIds(command, categories);
            var existingAuthors = categories.Where(author => categoriesDB.Contains(author.Id)).ToList();

            return existingAuthors;
        }

        private IEnumerable<SqlParameter> GetBookParameters(Book book)
        {
            var sqlCommands = new List<SqlParameter>()
            {
                new SqlParameter($"@{nameof(Book.Title)}", book.Title),
                new SqlParameter($"@{nameof(Book.Price)}", book.Price),
                new SqlParameter($"@{nameof(Book.DateOfPublication)}", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))),
                new SqlParameter($"@{nameof(Book.Description)}", book.Description ?? (object)DBNull.Value)
            };

            return sqlCommands;
        }

        private string DeleteBookCategories()
        {
            string deleteQuery = @"
                DELETE FROM BookCategory 
                WHERE BookId = @Id;";

            return deleteQuery;
        }

        private string DeleteBookAuthors()
        {
            string deleteQuery = @"
                DELETE FROM BookAuthor 
                WHERE BookId = @Id;";

            return deleteQuery;
        }

        private async Task AddBookEntities(Book book, SqlCommand command)
        {
            string authorQuery = string.Empty;
            string categoryQuery = string.Empty;

            if (book.Authors.Any())
            {
                var (query, parameters) = await AddAuthors(book.Id, book.Authors, command);

                authorQuery = query;

                foreach (var param in parameters)
                    command.Parameters.Add(param);
            }
            if (book.Categories.Any())
            {
                var (query, parameters) = await AddCategories(book.Id, book.Categories, command);

                categoryQuery = query;

                foreach (var param in parameters)
                    command.Parameters.Add(param);
            }

            command.CommandText = authorQuery + categoryQuery;

            await command.ExecuteNonQueryAsync();
        }

        private Author GetAuthor(SqlDataReader reader, int authorId)
        {
            var author = new Author
            {
                Id = authorId,
                FirstName = reader.GetString(nameof(Author.FirstName)),
                LastName = reader.GetString(nameof(Author.LastName)),
                Biography = reader.GetString(nameof(Author.Biography)),
            };

            return author;
        }

        private Category GetCategory(SqlDataReader reader, int categoryId)
        {
            var category = new Category
            {
                Id = categoryId,
                Name = reader.GetString(nameof(Category.Name))
            };

            return category;
        }
    }
}
