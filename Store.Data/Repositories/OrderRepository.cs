using Microsoft.Data.SqlClient;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;
using System.Data;
using System.Text;

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
                    string query = @"
                    SELECT 
                        o.Id,
                        o.Sum,
                        o.Date,
                        u.Id AS UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        oi.Id AS OrderItemsId,
                        oi.Amount,
                        oi.Price,
                        b.Id AS BookId,
                        b.Title
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
                    command.CommandText = query;
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@Id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    await sqlConnection.OpenAsync();

                    await GetOrderSql(command, order);
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
                    string query = @"
                    SELECT 
                        o.Id,
                        o.Sum,
                        o.Date,
                        u.Id AS UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        oi.Id AS OrderItemsId,
                        oi.Amount,
                        oi.Price,
                        b.Id AS BookId,
                        b.Title
                    FROM 
                        Orders o
                    JOIN 
                        Users u ON o.UserId = u.Id
                    LEFT JOIN 
                        OrderItems oi ON oi.OrderId = o.Id
                    LEFT JOIN 
                        Books b ON oi.BookId = b.Id";
                    command.CommandText = query;

                    await sqlConnection.OpenAsync();

                    await GetOrdersSql(command, orders);

                }
            }
            return orders;
        }
        private async Task GetOrderSql(SqlCommand command, Order order)
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    ReadOrder(reader, order);
                    ReadUser(reader, order);

                    order.OrderItems = new List<OrderItems>();
                    ReadOrderItems(reader, order);
                }
            }
        }
        private async Task GetOrdersSql(SqlCommand command, List<Order> orders)
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var orderId = reader.GetInt32(nameof(Order.Id));
                    var order = orders.FirstOrDefault(o => o.Id == orderId);

                    if (order == null)
                    {
                        order = new Order();
                        ReadOrder(reader, order);
                        ReadUser(reader, order);
                        order.OrderItems = new List<OrderItems>();
                        ReadOrderItems(reader, order);

                        orders.Add(order);
                    }                   
                }
            }
        }
        private void ReadOrder(SqlDataReader reader, Order order)
        {
            order.Id = reader.GetInt32(nameof(Order.Id));
            order.Sum = reader.GetDecimal(nameof(Order.Sum));
            order.Date = DateOnly.FromDateTime(reader.GetDateTime(nameof(Order.Date)));
        }
        private void ReadUser(SqlDataReader reader, Order order)
        {
            order.User = new User();
            order.User.Id = reader.GetInt32("UserId");
            order.User.FirstName = reader.GetString(nameof(User.FirstName));
            order.User.LastName = reader.GetString(nameof(User.LastName));
            order.User.Email = reader.GetString(nameof(User.Email));
        }
        private void ReadOrderItems(SqlDataReader reader, Order order)
        {
            if (!reader.IsDBNull("OrderItemsId"))
            {
                var orderItem = new OrderItems
                {
                    Id = reader.GetInt32("OrderItemsId"),
                    Amount = reader.GetInt32(nameof(OrderItems.Amount)),
                    Price = reader.GetDecimal(nameof(OrderItems.Price)),
                    Book = new Book
                    {
                        Id = reader.GetInt32("BookId"),
                        Title = reader.GetString(nameof(Book.Title))
                    }
                };
                order.OrderItems.Add(orderItem);
            }
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
                            var query = new StringBuilder(@"
                                INSERT INTO Orders (Sum, Date, UserId) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Sum, @Date, @UserId)");

                            command.Parameters.Add(new SqlParameter("@Sum", order.Sum));
                            command.Parameters.Add(new SqlParameter("@Date", order.Date));
                            command.Parameters.Add(new SqlParameter("@UserId", 1));

                            if (order.OrderItems != null)
                                AddOrderItems(query, command, order);

                            command.CommandText = query.ToString();

                            orderId = (int)await command.ExecuteScalarAsync();
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
            return orderId;
        }
        private void AddOrderItems(StringBuilder query, SqlCommand command, Order order)
        {
            foreach (var orderitems in order.OrderItems)
            {
                query.Append(@"
                    INSERT INTO OrderItems (Amount, Price, OrderId, BookId) 
                    VALUES (@Amount, @Price, @OrderId, @BookId)");
                command.Parameters.Add(new SqlParameter("@Amount", orderitems.Amount));
                command.Parameters.Add(new SqlParameter("@Price", orderitems.Price));
                command.Parameters.Add(new SqlParameter("@OrderId", order.Id));
                command.Parameters.Add(new SqlParameter("@BookId", orderitems.Book.Id));
            }
        }
        public async Task<bool> UpdateAsync(Order order)
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
                            var query = new StringBuilder(@"
                                UPDATE Orders
                                SET Sum = @Sum,
                                    Date = @Date,
                                    UserId = @UserId
                                WHERE Id = @Id");

                            command.Parameters.Add(new SqlParameter("@Sum", order.Sum));
                            command.Parameters.Add(new SqlParameter("@Date", order.Date.ToDateTime(new TimeOnly(0, 0))));
                            command.Parameters.Add(new SqlParameter("@UserId", 2));
                            command.Parameters.Add(new SqlParameter("@Id", order.Id));

                            if (order.OrderItems != null)
                                AddOrderItems(query, command, order);

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

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            string query = @"
                                DELETE FROM Orders
                                WHERE Id = @Id";
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
    }
}
