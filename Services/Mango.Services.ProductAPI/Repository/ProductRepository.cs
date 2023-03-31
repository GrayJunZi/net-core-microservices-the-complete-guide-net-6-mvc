using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.DTOs;
using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var productList = await _dbContext.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(productList);
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<ProductDto, Product>(productDto);
            if (product == null)
                return null;

            if (product.ProductId > 0)
            {
                _dbContext.Products.Update(product);
            }
            else
            {
                _dbContext.Products.Add(product);
            }
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
                return false;

            _dbContext.Products.Remove(product);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
