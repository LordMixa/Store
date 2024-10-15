using Microsoft.Data.SqlClient;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;
using System.Data;

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
            Book book = new Book();
            book.Authors = new List<Author>();
            book.Categories = new List<Category>();

            if (_dbConnection is SqlConnection sqlConnection)
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"
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
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    await sqlConnection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            book.Id = reader.GetInt32(0);
                            book.Title = reader.GetString(1);
                            book.Price = reader.GetDecimal(2);
                            book.DateOfPublication = DateOnly.FromDateTime(reader.GetDateTime(3));
                            book.Description = reader.GetString(4);

                            do
                            {
                                if (!reader.IsDBNull(5) && !book.Authors.Any(a=>a.Id == reader.GetInt32(5)))
                                {
                                    var author = new Author
                                    {
                                        Id = reader.GetInt32(5),
                                        FirstName = reader.GetString(6),
                                        LastName = reader.GetString(7),
                                        Biography = reader.IsDBNull(8) ? null : reader.GetString(7)
                                    };
                                    book.Authors.Add(author);
                                }

                                if (!reader.IsDBNull(9))
                                {
                                    var category = new Category
                                    {
                                        Id = reader.GetInt32(9),
                                        Name = reader.GetString(10)
                                    };
                                    book.Categories.Add(category);
                                }

                            } while (await reader.ReadAsync());
                        }
                    }
                }
            }
            return book;
        }
        public async Task<IEnumerable<Book>> GetAsync()
        {
            var books = new List<Book>();

            if (_dbConnection is SqlConnection sqlConnection)
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"
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

                    await sqlConnection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var bookId = reader.GetInt32(0);
                            var book = books.FirstOrDefault(b => b.Id == bookId);

                            if (book == null)
                            {
                                book = new Book
                                {
                                    Id = bookId,
                                    Title = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    DateOfPublication = DateOnly.FromDateTime(reader.GetDateTime(3)),
                                    Description = reader.GetString(4),
                                    Authors = new List<Author>(),
                                    Categories = new List<Category>()
                                };

                                books.Add(book);
                            }

                            if (!reader.IsDBNull(5) && !book.Authors.Any(a => a.Id == reader.GetInt32(5)))
                            {
                                var author = new Author
                                {
                                    Id = reader.GetInt32(5),
                                    FirstName = reader.GetString(6),
                                    LastName = reader.GetString(7),
                                    Biography = reader.IsDBNull(8) ? null : reader.GetString(8)
                                };
                                book.Authors.Add(author);
                            }

                            if (!reader.IsDBNull(9) && !book.Categories.Any(c => c.Id == reader.GetInt32(9)))
                            {
                                var category = new Category
                                {
                                    Id = reader.GetInt32(9),
                                    Name = reader.GetString(10)
                                };
                                book.Categories.Add(category);
                            }
                        }
                    }
                }
            }

            return books;
        }
        public async Task<int> CreateAsync(Book book)
        {
            int bookId = default;

            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO Books (Title, Price, DateOfPublication, Description) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Title, @Price, @DateOfPublication, @Description)";

                            command.Parameters.Add(new SqlParameter("@Title", book.Title));
                            command.Parameters.Add(new SqlParameter("@Price", book.Price));
                            command.Parameters.Add(new SqlParameter("@DateOfPublication", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@Description", book.Description ?? (object)DBNull.Value));

                            bookId = (int)await command.ExecuteScalarAsync();
                        }
                        if (book.Authors != null)
                        {
                            foreach (var author in book.Authors)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO BookAuthor (BookId, AuthorId) 
                                    VALUES (@BookId, @AuthorId)";

                                    command.Parameters.Add(new SqlParameter("@BookId", bookId));
                                    command.Parameters.Add(new SqlParameter("@AuthorId", author.Id));

                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        if (book.Categories != null)
                        {
                            foreach (var category in book.Categories)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO BookCategory (BookId, CategoryId) 
                                    VALUES (@BookId, @CategoryId)";

                                    command.Parameters.Add(new SqlParameter("@BookId", bookId));
                                    command.Parameters.Add(new SqlParameter("@CategoryId", category.Id));

                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                        }
                        transaction.Commit();
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
        public async Task<bool> UpdateAsync(Book book, int id)
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                UPDATE Books
                                SET Title = @Title,
                                    Price = @Price,
                                    DateOfPublication = @DateOfPublication,
                                    Description = @Description
                                WHERE Id = @Id";

                            command.Parameters.Add(new SqlParameter("@Title", book.Title));
                            command.Parameters.Add(new SqlParameter("@Price", book.Price));
                            command.Parameters.Add(new SqlParameter("@DateOfPublication", book.DateOfPublication.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@Description", book.Description ?? (object)DBNull.Value));
                            command.Parameters.Add(new SqlParameter("@Id", id));

                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM BookAuthor
                                WHERE BookId = @BookId";

                            command.Parameters.Add(new SqlParameter("@BookId", id));
                            await command.ExecuteNonQueryAsync();
                        }
                        if (book.Authors != null)
                        {
                            foreach (var author in book.Authors)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO BookAuthor (BookId, AuthorId)
                                    VALUES (@BookId, @AuthorId)";

                                    command.Parameters.Add(new SqlParameter("@BookId", id));
                                    command.Parameters.Add(new SqlParameter("@AuthorId", author.Id));

                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                        }
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM BookCategory
                                WHERE BookId = @BookId";

                            command.Parameters.Add(new SqlParameter("@BookId", id));
                            await command.ExecuteNonQueryAsync();
                        }
                        if (book.Categories != null)
                        {
                            foreach (var category in book.Categories)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO BookCategory (BookId, CategoryId)
                                    VALUES (@BookId, @CategoryId)";

                                    command.Parameters.Add(new SqlParameter("@BookId", id));
                                    command.Parameters.Add(new SqlParameter("@CategoryId", category.Id));

                                    await command.ExecuteNonQueryAsync();
                                }
                            }
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
            return false;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            if (_dbConnection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM BookAuthor
                                WHERE BookId = @BookId";

                            command.Parameters.Add(new SqlParameter("@BookId", id));
                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM BookCategory
                                WHERE BookId = @BookId";

                            command.Parameters.Add(new SqlParameter("@BookId", id));
                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM Books
                                WHERE Id = @Id";

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
    }
}
