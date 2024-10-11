using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Models.ProductModels;
using Store.Data.Entities;

namespace Store.Business.MapperConfigurations
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile() 
        {
            CreateMap<Order, OrderModel>()
                .ForMember(dest => dest.PriceEuro, opt => opt.MapFrom(src => src.Price / 1.2))
                .ForMember(dest => dest.PriceUah, opt => opt.MapFrom(src => src.Price * 41))
                .ForMember(dest => dest.PriceUsd, opt => opt.MapFrom(src => src.Price));

            CreateMap<Product, ProductModel>();
        }
    }
}
