using Microsoft.Data.SqlClient;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;
using System.Data;
using static System.Reflection.Metadata.BlobBuilder;

namespace Store.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _dbConnection;

        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Order> GetAsync(int id)
        {
            Order order = new Order();

            if (_dbConnection is SqlConnection sqlConnection)
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT 
                        o.Id,
                        o.Sum,
                        o.Date,
                        u.Id AS UserId,
                        u.FirstName AS UserFirstName,
                        u.LastName AS UserLastName,
                        u.Email AS UserEmail,
                        oi.Id AS OrderItemId,
                        oi.Amount AS OrderItemAmount,
                        oi.Price AS OrderItemPrice,
                        b.Id AS BookId,
                        b.Title AS BookTitle
                    FROM 
                        Orders o
                    JOIN 
                        Users u ON o.UserId = u.Id
                    LEFT JOIN 
                        OrderItems oi ON oi.OrderId = o.Id
                    LEFT JOIN 
                        Books b ON oi.BookId = b.Id
                    WHERE 
                        o.Id = @Id;";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    await sqlConnection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            order.Id = reader.GetInt32(0);
                            order.Sum = reader.GetDecimal(1);
                            order.Date = DateOnly.FromDateTime(reader.GetDateTime(2));
                            order.User = new User
                            {
                                Id = reader.GetInt32(3),
                                FirstName = reader.GetString(4),
                                LastName = reader.GetString(5),
                                Email = reader.GetString(6)
                            };
                            order.OrderItems = new List<OrderItems>();

                            do
                            {
                                if (!reader.IsDBNull(7))
                                {
                                    var orderItem = new OrderItems
                                    {
                                        Id = reader.GetInt32(7),
                                        Amount = reader.GetInt32(8),
                                        Price = reader.GetDecimal(9),
                                        Book = new Book
                                        {
                                            Id = reader.GetInt32(10),
                                            Title = reader.GetString(11)
                                        }
                                    };
                                    order.OrderItems.Add(orderItem);
                                }

                            } while (await reader.ReadAsync());
                        }
                    }
                }
            }
            return order;
        }
        public async Task<IEnumerable<Order>> GetAsync()
        {
            var orders = new List<Order>();

            if (_dbConnection is SqlConnection sqlConnection)
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT 
                        o.Id,
                        o.Sum,
                        o.Date,
                        u.Id AS UserId,
                        u.FirstName AS UserFirstName,
                        u.LastName AS UserLastName,
                        u.Email AS UserEmail,
                        oi.Id AS OrderItemId,
                        oi.Amount AS OrderItemAmount,
                        oi.Price AS OrderItemPrice,
                        b.Id AS BookId,
                        b.Title AS BookTitle
                    FROM 
                        Orders o
                    JOIN 
                        Users u ON o.UserId = u.Id
                    LEFT JOIN 
                        OrderItems oi ON oi.OrderId = o.Id
                    LEFT JOIN 
                        Books b ON oi.BookId = b.Id";

                    await sqlConnection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var orderId = reader.GetInt32(0);
                            var order = orders.FirstOrDefault(o => o.Id == orderId);

                            if (order == null)
                            {
                                order = new Order
                                {
                                    Id = reader.GetInt32(0),
                                    Sum = reader.GetDecimal(1),
                                    Date = DateOnly.FromDateTime(reader.GetDateTime(2)),
                                    User = new User
                                    {
                                        Id = reader.GetInt32(3),
                                        FirstName = reader.GetString(4),
                                        LastName = reader.GetString(5),
                                        Email = reader.GetString(6)
                                    },
                                    OrderItems = new List<OrderItems>()
                                };
                                orders.Add(order);
                            }

                            if (!reader.IsDBNull(7))
                            {
                                var orderItem = new OrderItems
                                {
                                    Id = reader.GetInt32(7),
                                    Amount = reader.GetInt32(8),
                                    Price = reader.GetDecimal(9),
                                    Book = new Book
                                    {
                                        Id = reader.GetInt32(10),
                                        Title = reader.GetString(11)
                                    }
                                };
                                order.OrderItems.Add(orderItem);
                            }
                        }
                    }
                }
            }
            return orders;
        }
        public async Task<int> CreateAsync(Order order)
        {
            int orderId = default;

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
                                INSERT INTO Orders (Sum, Date, UserId) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Sum, @Date, @UserId)";

                            command.Parameters.Add(new SqlParameter("@Sum", order.Sum));
                            command.Parameters.Add(new SqlParameter("@Date", order.Date));
                            command.Parameters.Add(new SqlParameter("@UserId", 1));

                            orderId = (int)await command.ExecuteScalarAsync();
                        }
                        if (order.OrderItems != null)
                        {
                            foreach (var orderitems in order.OrderItems)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO OrderItems (Amount, Price, OrderId, BookId) 
                                    VALUES (@Amount, @Price, @OrderId, @BookId)";

                                    command.Parameters.Add(new SqlParameter("@Amount", orderitems.Amount));
                                    command.Parameters.Add(new SqlParameter("@Price", orderitems.Price));
                                    command.Parameters.Add(new SqlParameter("@OrderId", order.Id));
                                    command.Parameters.Add(new SqlParameter("@BookId", orderitems.Book.Id));

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

            return orderId;
        }
        public async Task<bool> UpdateAsync(Order order, int id)
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
                                UPDATE Orders
                                SET Sum = @Sum,
                                    Date = @Date,
                                    UserId = @UserId
                                WHERE Id = @Id";

                            command.Parameters.Add(new SqlParameter("@Sum", order.Sum));
                            command.Parameters.Add(new SqlParameter("@Date", order.Date.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@UserId", 2));
                            command.Parameters.Add(new SqlParameter("@Id", id));

                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM OrderItems
                                WHERE OrderId = @OrderId";

                            command.Parameters.Add(new SqlParameter("@OrderId", id));
                            await command.ExecuteNonQueryAsync();
                        }
                        if (order.OrderItems != null)
                        {
                            foreach (var orderItems in order.OrderItems)
                            {
                                using (var command = sqlConnection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = @"
                                    INSERT INTO OrderItems (BookId, OrderId, Amount, Price)
                                    VALUES (@BookId, @OrderId, @Amount, @Price)";

                                    command.Parameters.Add(new SqlParameter("@BookId", orderItems.Book.Id));
                                    command.Parameters.Add(new SqlParameter("@OrderId", id));
                                    command.Parameters.Add(new SqlParameter("@Amount", orderItems.Amount));
                                    command.Parameters.Add(new SqlParameter("@Price", orderItems.Price));

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
                                DELETE FROM OrderItems
                                WHERE OrderId = @OrderId";

                            command.Parameters.Add(new SqlParameter("@OrderId", id));
                            await command.ExecuteNonQueryAsync();
                        }

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                DELETE FROM Orders
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
