using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.DTOs;
using Mango.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<CartDto> GetCartByUserId(string userId)
        {
            var cart = new Cart
            {
                CartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
            };

            cart.CartDetails = _dbContext.CartDetails
                .Where(x => x.CartHeaderId == cart.CartHeader.Id)
                .Include(x => x.Product);

            return _mapper.Map<CartDto>(cart);
        }
        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == cartDto.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();
            }

            var cartHeader = await _dbContext.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == cart.CartHeader.UserId);

            if (cartHeader == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();

                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetail = await _dbContext.CartDetails.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                    && x.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                    _dbContext.CartDetails.Update(cartDetail);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }
        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                var cartDetail = await _dbContext.CartDetails
                    .FirstOrDefaultAsync(x => x.Id == cartDetailId);

                var totalCountOfCartItems = _dbContext.CartDetails
                    .Where(x => x.CartHeaderId == cartDetail.CartHeaderId).Count();

                _dbContext.CartDetails.Remove(cartDetail);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dbContext.CartHeaders
                        .FirstOrDefaultAsync(x => x.Id == cartDetail.CartHeaderId);

                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cartHeader != null)
            {
                _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails.Where(x => x.CartHeaderId == cartHeader.Id));
                _dbContext.CartHeaders.Remove(cartHeader);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
