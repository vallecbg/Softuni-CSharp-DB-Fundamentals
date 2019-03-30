using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDto, User>();

            //6
            CreateMap<User, GetSoldProductsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(c => c.ProductsSold));

            CreateMap<Product, ProductsSoldDto>();

            //7
            CreateMap<Category, GetCategoriesByProductsCountDto>()
                .ForMember(x => x.Count, y => y.MapFrom(c => c.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(c => c.CategoryProducts.Average(x => x.Product.Price)))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(c => c.CategoryProducts.Sum(x => x.Product.Price)));

            //8

            CreateMap<ICollection<UserDto>, UsersAndProductsDto>()
                .ForMember(x => x.Users, y => y.MapFrom(obj => obj.Take(10)))
                .ForMember(x => x.Count, y => y.MapFrom(obj => obj.Count));

            CreateMap<User, UserDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(obj => obj.ProductsSold));

            CreateMap<User, SoldProductsDto>()
                .ForMember(x => x.Products, y => y.MapFrom(obj => obj.ProductsSold));

            CreateMap<ICollection<Product>, SoldProductsDto>()
                .ForMember(x => x.Products, y => y.MapFrom(obj => obj.OrderByDescending(z => z.Price)))
                .ForMember(x => x.Count, y => y.MapFrom(obj => obj.Count));
        }
    }
}
