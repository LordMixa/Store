﻿using AutoMapper;
using Store.Business.Models.OrderModels;
using Store.Business.Models.BookModels;
using Store.Business.Models.OrderItemsModels;
using Store.Business.Models.UserModels;
using Store.Business.Models.AuthorModels;
using Store.Business.Models.CategoryModels;
using Store.Contracts.Responses.Orders;
using Store.Contracts.Responses.Books;
using Store.Contracts.Responses.OrderItems;
using Store.Contracts.Responses.Users;
using Store.Contracts.Responses.Authors;
using Store.Contracts.Responses.Categories;
using Store.Contracts.Requests.Books;
using Store.Contracts.Requests.Orders;

namespace Store.Api.MapperConfigurations
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile() 
        {
            CreateMap<OrderModel, OrderResponseModel>();
            CreateMap<BookModel, BookResponseModel>();
            CreateMap<BookModel, BookTitleResponseModel>();
            CreateMap<OrderItemsModel, OrderItemsResponseModel>();
            CreateMap<UserModel, UserResponseModel>();
            CreateMap<AuthorModel, AuthorResponseModel>();
            CreateMap<CategoryModel, CategoryResponseModel>();

            CreateMap<BookCreateRequestModel, BookCreateModel>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
                    src.AuthorIds.Select(authorId => new AuthorModel { Id = authorId }).ToList()))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.CategoryIds.Select(categoryId => new CategoryModel { Id = categoryId }).ToList()));

            CreateMap<OrderCreateRequestModel, OrderCreateModel>();
        }
    }
}
