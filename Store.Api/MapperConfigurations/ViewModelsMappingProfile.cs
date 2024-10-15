using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Models.BookModels;
using Store.ViewModels.ViewModels.OrderViewModels;
using Store.ViewModels.ViewModels.BookViewModels;
using Store.Api.Models.Requests;
using Store.Business.Models.OrderItemsModels;
using Store.ViewModels.ViewModels.OrderItemsViewModels;
using Store.Business.Models.UserModels;
using Store.ViewModels.ViewModels.UserViewModels;

namespace Store.Api.MapperConfigurations
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile() 
        {
            CreateMap<OrderModel, OrderViewModel>();
            CreateMap<BookModel, BookViewModel>();
            CreateMap<OrderItemsModel, OrderItemsViewModel>();
            CreateMap<UserModel, UserViewModel>();

            CreateMap<BookRequestModel, BookCreateModel>();
            CreateMap<OrderRequestModel, OrderCreateModel>();
        }
    }
}
