using AutoMapper;
using ProductManagement.App.DTOs;
using ProductManagement.Core.Entities;

namespace ProductManagement.App.Profiles
{
    public class ProductMappingProfile: Profile
    {
        public ProductMappingProfile() 
        { 
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
        }
    }
}
