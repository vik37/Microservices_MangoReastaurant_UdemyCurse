using AutoMapper;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMapp()
        {
            var mapppingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<ProductDto, Product>();
                //config.CreateMap<Product, ProductDto>();
            });
            return mapppingConfig;
        }
    }
}
