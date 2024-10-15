using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Models.BookModels;
using Store.Data.Entities;
using Store.Business.Models.OrderItemsModels;
using Store.Business.Models.UserModels;
using Store.Business.Models.CategoryModels;
using Store.Business.Models.AuthorModels;
using Store.Business.Models.BookDetailsModels;

namespace Store.Business.MapperConfigurations
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile() 
        {
            CreateMap<Order, OrderModel>();
            CreateMap<Book, BookModel>();
            CreateMap<OrderItems, OrderItemsModel>();
            CreateMap<User, UserModel>();
            CreateMap<Category, CategoryModel>();
            CreateMap<Author, AuthorModel>();
            CreateMap<BookDetails, BookDetailsModel>();


            CreateMap<BookCreateModel, Book>();
            CreateMap<OrderCreateModel, Order>();
        }
    }
}
