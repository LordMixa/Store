using Dapper;
using Store.Data.Dapper.Repositories.Interfaces;
using Store.Entities.Dtos;
using Store.Entities.Entities;
using System.Data;

namespace Store.Data.Dapper.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public async Task<Book?> GetAsync(int id)
        {
            using (_sqlConnection)
            {
                var parameters = new { @Id = id };

                var booksDictionary = new Dictionary<int, Book>();

                await _sqlConnection.QueryAsync<Book, Author, Category, Book>(
                "Procedure_GetBookById",
                (book, author, category) =>
                {

                      if (booksDictionary.TryGetValue(book.Id, out var existingBook))
                      {
                          book = existingBook;
                      }
                      else
                      {
                          booksDictionary.Add(book.Id, book);
                      }

                      if (author != null && !book.Authors.Any(a => a.Id == author.Id))
                      {
                          book.Authors.Add(author);
                      }
                      if (category != null && !book.Categories.Any(c => c.Id == category.Id))
                      {
                          book.Categories.Add(category);
                      }

                      return book;
                },
                parameters,
                splitOn: "AuthorId,CategoryId",
                commandType: CommandType.StoredProcedure
                );

                var bookTryGetBool = booksDictionary.TryGetValue(id, out var bookResponse);

                return bookResponse;
            }
        }

        public async Task<IEnumerable<BookDto>?> GetAsync()
        {
            using (_sqlConnection)
            {
                var books = await _sqlConnection.QueryAsync<BookDto>(
                "Procedure_GetAllBooks",
                commandType: CommandType.StoredProcedure);

                return books;
            }
        }

        public async Task<int> CreateAsync(Book book)
        {
            var authorIds = book.Authors.Select(a => a.Id);
            var categoryIds = book.Categories.Select(c => c.Id);

            var authors = CreateEntityIdsTable(authorIds);
            var categories = CreateEntityIdsTable(categoryIds);

            using (_sqlConnection)
            {
                var parameters = new 
                { 
                    book.Title, 
                    book.Description, 
                    book.Price, 
                    book.DateOfPublication, 
                    @AuthorIds = authors, 
                    @CategoryIds = categories 
                };

                int id = await _sqlConnection.QuerySingleOrDefaultAsync<int>(
                "Procedure_CreateBook",
                parameters,
                commandType: CommandType.StoredProcedure
                );

                return id;
            }
        }

        private DataTable CreateEntityIdsTable(IEnumerable<int> ids)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Id", typeof(int));

            foreach (var id in ids)
            {
                dataTable.Rows.Add(id);
            }

            return dataTable;
        }
    }
}