using AutoMapper;
using Store.Business.Models.BookModels;
using Store.Business.Models.OrderItems;
using Store.Business.Models.Orders;
using Store.Business.Models.Users;
using Store.Business.Services.Interfaces;
using Store.Data.Dtos;
using Store.Data.Entities;
using Store.Data.Repositories.Interfaces;

namespace Store.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<OrderModel> GetAsync(int id)
        {
            var order = await _orderRepository.GetAsync(id);
            var orderModel = _mapper.Map<OrderModel>(order);

            return orderModel;
        }

        public async Task<IEnumerable<OrderModel>> GetAsync()
        {
            var orderDtos = await _orderRepository.GetAsync();
            var orderModels = DtoMapToModel(orderDtos);

            return orderModels;
        }

        public async Task<int> CreateAsync(OrderCreateModel orderModel)
        {
            var order = _mapper.Map<Order>(orderModel);
            var id = await _orderRepository.CreateAsync(order);

            return id;
        }

        public async Task<bool> UpdateAsync(OrderCreateModel orderModel)
        {
            var order = _mapper.Map<Order>(orderModel);
            var isSuccess = await _orderRepository.UpdateAsync(order);

            return isSuccess;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isSuccess = await _orderRepository.DeleteAsync(id);

            return isSuccess;
        }

        private IEnumerable<OrderModel> DtoMapToModel(IEnumerable<OrderDto> orderDtos)
        {
            var orders = orderDtos
                .GroupBy(dto => dto.Id)
                .Select(group =>
                {
                    var firstDto = group.FirstOrDefault();

                    var orderModel = new OrderModel
                    {
                        Id = group.Key,
                        Sum = firstDto.Sum,
                        Date = firstDto.Date,
                        User = new UserModel
                        {
                            FirstName = firstDto.FirstName,
                            LastName = firstDto.LastName,
                            Email = firstDto.Email,
                            Id = firstDto.UserId,
                        },
                        OrderItems = group
                            .Select(dto => new OrderItemsModel
                            {
                                Id = dto.OrderItemsId,
                                Price = dto.Price,
                                Amount = dto.Amount,
                                Book = new BookModel
                                {
                                    Id = dto.BookId,
                                    Title = dto.Title
                                }
                            })
                            .DistinctBy(item => item.Id)
                            .ToList()
                    };

                    return orderModel;
                });

            return orders;
        }
    }
}
