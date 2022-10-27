using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        public async Task<bool> ClearCart(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cart)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            throw new NotImplementedException();
        }
    }
}
