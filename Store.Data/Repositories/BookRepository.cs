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

        public async Task<Book> GetAsync(int id)
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
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
                        c.Name AS CategoryName
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
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    await sqlConnection.OpenAsync();

                    var book = await ReadBookSql(command);
                    return book;
                }
            }
            return null;
        }

        public async Task<IEnumerable<BookDto>> GetAsync()
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
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
                        c.Name AS CategoryName
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
            return null;
        }

        public async Task<int> CreateAsync(Book book)
        {
            int bookId = default;

            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                var query = new StringBuilder(@"
                                INSERT INTO Books (Title, Price, DateOfPublication, Description) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Title, @Price, @DateOfPublication, @Description)");

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.Parameters.Add(new SqlParameter("@Title", book.Title));
                            command.Parameters.Add(new SqlParameter("@Price", book.Price));
                            command.Parameters.Add(new SqlParameter("@DateOfPublication", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@Description", book.Description ?? (object)DBNull.Value));

                            if (book.Authors != null)
                                AddAuthors(query, command, book);
                            if (book.Categories != null)
                                AddCategories(query, command, book);

                            command.CommandText = query.ToString();

                            bookId = (int)await command.ExecuteScalarAsync();

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return bookId;
        }

        public async Task<bool> UpdateAsync(Book book)
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                var query = new StringBuilder(@"
                                UPDATE Books
                                SET Title = @Title,
                                    Price = @Price,
                                    DateOfPublication = @DateOfPublication,
                                    Description = @Description
                                WHERE Id = @Id");

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.Parameters.Add(new SqlParameter("@Title", book.Title));
                            command.Parameters.Add(new SqlParameter("@Price", book.Price));
                            command.Parameters.Add(new SqlParameter("@DateOfPublication", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@Description", book.Description ?? (object)DBNull.Value));
                            command.Parameters.Add(new SqlParameter("@Id", book.Id));

                            if (book.Authors != null)
                                AddAuthors(query, command, book);
                            if (book.Categories != null)
                                AddCategories(query, command, book);

                            command.CommandText = query.ToString();

                            await command.ExecuteNonQueryAsync();

                            transaction.Commit();
                            return true;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                string query = @"
                                DELETE FROM Books
                                WHERE Id = @Id";

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            command.CommandText = query;
                            command.Parameters.Add(new SqlParameter("@Id", id));
                            var affectedRows = await command.ExecuteNonQueryAsync();

                            if (affectedRows > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

            return false;
        }

        private async Task<Book> ReadBookSql(SqlCommand command)
        {
            var book = new Book();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ReadBook(reader, book);

                    if (!reader.IsDBNull("AuthorId") && !book.Authors.Any(a => a.Id == reader.GetInt32("AuthorId")))
                        book.Authors = ReadAuthors(reader);
                    if (!reader.IsDBNull("CategoryId") && !book.Categories.Any(c => c.Id == reader.GetInt32(reader.GetOrdinal("CategoryId"))))
                        book.Categories = ReadCategories(reader);
                }
            }
            return book;
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
                        CategoryName = reader.IsDBNull(reader.GetOrdinal(nameof(BookDto.CategoryName))) ? string.Empty : reader.GetString(nameof(BookDto.CategoryName))
                    };

                    bookDtos.Add(bookDto);
                }
            }
           
            return bookDtos;
        }

        private void ReadBook(SqlDataReader reader, Book book)
        {
            book.Id = reader.GetInt32(nameof(Book.Id));
            book.Title = reader.GetString(nameof(Book.Title));
            book.Price = reader.GetDecimal(nameof(Book.Price));
            book.DateOfPublication = DateOnly.FromDateTime(reader.GetDateTime(nameof(Book.DateOfPublication)));
            book.Description = reader.GetString(nameof(Book.Description));
        }

        private List<Author> ReadAuthors(SqlDataReader reader)
        {
            var authors = new List<Author>();
            var author = new Author
            {
                Id = reader.GetInt32("AuthorId"),
                FirstName = reader.GetString(nameof(Author.FirstName)),
                LastName = reader.GetString(nameof(Author.LastName)),
                Biography = reader.GetString(nameof(Author.Biography)),
            };

            authors.Add(author);

            return authors;
        }

        private List<Category> ReadCategories(SqlDataReader reader)
        {
            var categories = new List<Category>();
            var category = new Category
            {
                Id = reader.GetInt32("CategoryId"),
                Name = reader.GetString("CategoryName")
            };

            categories.Add(category);

            return categories;
        }

        private void AddAuthors(StringBuilder query, SqlCommand command, Book book)
        {
            foreach (var author in book.Authors)
            {
                query.Append(@"
                    INSERT INTO BookAuthor (BookId, AuthorId) 
                    VALUES (@BookId, @AuthorId)");
                command.Parameters.Add(new SqlParameter("@BookId", book.Id));
                command.Parameters.Add(new SqlParameter("@AuthorId", author.Id));
            }
        }

        private void AddCategories(StringBuilder query, SqlCommand command, Book book)
        {
            foreach (var category in book.Categories)
            {
                query.Append(@"
                    INSERT INTO BookCategory (BookId, CategoryId) 
                    VALUES (@BookId, @CategoryId)");

                command.Parameters.Add(new SqlParameter("@BookId", book.Id));
                command.Parameters.Add(new SqlParameter("@CategoryId", category.Id));
            }
        }
    }
}
