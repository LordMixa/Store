using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Services.Interfaces;
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
            var orders = await _orderRepository.GetAsync();
            var orderModels = _mapper.Map<List<OrderModel>>(orders);

            return orderModels;
        }
        public async Task<int> CreateAsync(OrderCreateModel orderModel)
        {
            var order = _mapper.Map<Order>(orderModel);
            var id = await _orderRepository.CreateAsync(order);

            return id;
        }
        public async Task<bool> UpdateAsync(OrderCreateModel orderModel, int id)
        {
            var order = _mapper.Map<Order>(orderModel);
            var isSucces = await _orderRepository.UpdateAsync(order, id);

            return isSucces;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var isSucces = await _orderRepository.DeleteAsync(id);

            return isSucces;
        }
    }
}
