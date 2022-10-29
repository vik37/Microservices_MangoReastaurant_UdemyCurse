using Mango.Web.Models;
using Mango.Web.Models.ShoppingCartModels;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        private readonly IHttpClientFactory _clientFactory;
        public CartService(IHttpClientFactory clientFactory)
            : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCard/" + userId,
                AccessToken = token
            });
        }
        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/AddCart",
                AccessToken = token
            });
        }
        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/UpdateCart",
                AccessToken = token
            });
        }
        public async Task<T> DeleteCartAsync<T>(int cartId, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCouponAsync<T>(string userId, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = userId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }
        public async Task<T> Checkout<T>(CartHeaderDto cartHeader, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeader,
                Url = SD.ShoppingCartAPIBase + "/api/cart/checkout",
                AccessToken = token
            });
        }
    }
}
