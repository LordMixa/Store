using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Services.Interfaces;
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
        public OrderModel Get(int id)
        {
            var order = _orderRepository.Get(id);
            var orderModel = _mapper.Map<OrderModel>(order);

            return orderModel;
        }
    }
}
