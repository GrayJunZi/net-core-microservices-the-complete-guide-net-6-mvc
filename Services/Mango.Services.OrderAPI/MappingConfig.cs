using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.ProductAPI.DTOs;

namespace Mango.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<OrderHeader, OrderHeader>();
            config.CreateMap<OrderHeader, ProductDto>();

        });
        return mappingConfig;
    }
}