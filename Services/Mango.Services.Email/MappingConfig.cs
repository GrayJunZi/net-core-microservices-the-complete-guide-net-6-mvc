using AutoMapper;
using Mango.Services.Email.Models;

namespace Mango.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {


        });
        return mappingConfig;
    }
}