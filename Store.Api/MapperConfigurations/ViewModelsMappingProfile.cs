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
using Store.Business.Models.AuthorModels;
using Store.ViewModels.ViewModels.AuthorViewModels;
using Store.Business.Models.CategoryModels;
using Store.ViewModels.ViewModels.CategoryViewModels;

namespace Store.Api.MapperConfigurations
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile() 
        {
            CreateMap<OrderModel, OrderViewModel>();
            CreateMap<BookModel, BookViewModel>();
            CreateMap<BookModel, BookTitleViewModel>();
            CreateMap<OrderItemsModel, OrderItemsViewModel>();
            CreateMap<UserModel, UserViewModel>();
            CreateMap<AuthorModel, AuthorViewModel>();
            CreateMap<CategoryModel, CategoryViewModel>();

            CreateMap<BookRequestModel, BookCreateModel>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
                    src.AuthorIds.Select(authorId => new AuthorModel { AuthorId = authorId }).ToList()))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.CategoryIds.Select(categoryId => new CategoryModel { CategoryId = categoryId }).ToList()));

            CreateMap<OrderRequestModel, OrderCreateModel>();
        }
    }
}
