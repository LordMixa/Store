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
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetAsync(id);
            var orderViewModel = _mapper.Map<OrderViewModel>(order);

            return Ok(orderViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAsync();
            var orderViewModels = _mapper.Map<List<OrderViewModel>>(orders);

            return Ok(orderViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderRequestModel request)
        {
            var orderModel = _mapper.Map<OrderCreateModel>(request);
            int id = await _orderService.CreateAsync(orderModel);

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderRequestModel request)
        {
            var orderModel = _mapper.Map<OrderCreateModel>(request);
            orderModel.Id = id;
            var isSuccess = await _orderService.UpdateAsync(orderModel);

            return Ok(isSuccess);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _orderService.DeleteAsync(id);

            return Ok(isSuccess);
        }
    }
}
