using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Models.ProductModels;
using Store.ViewModels.ViewModels.OrderViewModels;
using Store.ViewModels.ViewModels.ProductViewModels;

namespace Store.Api.MapperConfigurations
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile() 
        {
            CreateMap<ProductModel, ProductViewModel>();
            CreateMap<OrderModel, OrderViewModel>();
        }
    }
}
