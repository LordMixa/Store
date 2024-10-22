using AutoMapper;
using Store.Business.Models.Orders;
using Store.Business.Models.BookModels;
using Store.Business.Models.OrderItems;
using Store.Business.Models.Users;
using Store.Business.Models.Categories;
using Store.Business.Models.Authors;
using Store.Business.Models.BookDetails;
using Store.Entities.Entities;

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
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<Author, AuthorModel>().ReverseMap();
            CreateMap<BookDetails, BookDetailsModel>();

            CreateMap<BookCreateModel, Book>();
            CreateMap<OrderCreateModel, Order>();
        }
    }
}
