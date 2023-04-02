using AutoMapper;
using Mango.Services.ShoppingCartAPI.DTOs;
using Mango.Services.ShoppingCartAPI.Models;

namespace Mango.Services.ShoppingCartAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductDto, Product>().ReverseMap();
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            config.CreateMap<CartDto, Cart>().ReverseMap();
        });
        return mappingConfig;
    }
}