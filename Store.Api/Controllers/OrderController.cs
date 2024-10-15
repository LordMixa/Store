using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models.Requests;
using Store.Business.Models.OrderModels;
using Store.Business.Services.Interfaces;
using Store.ViewModels.ViewModels.OrderViewModels;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<OrderViewModel> Get(int id)
        {
            var order = await _orderService.GetAsync(id);
            var orderViewModel = _mapper.Map<OrderViewModel>(order);

            return orderViewModel;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> Get()
        {
            var orders = await _orderService.GetAsync();
            var orderViewModels = _mapper.Map<List<OrderViewModel>>(orders);

            return orderViewModels;

        }
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] OrderRequestModel request)
        {
            var orderModel = _mapper.Map<OrderCreateModel>(request);
            int id = await _orderService.CreateAsync(orderModel);

            return id;

        }
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] OrderRequestModel request)
        {
            var orderModel = _mapper.Map<OrderCreateModel>(request);
            var isSuccess = await _orderService.UpdateAsync(orderModel, id);

            return isSuccess;

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var isSuccess = await _orderService.DeleteAsync(id);

            return isSuccess;
        }
    }
}
