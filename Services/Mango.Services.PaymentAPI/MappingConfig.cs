using AutoMapper;

namespace Mango.Services.PaymentAPI;

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