using Mango.Web.Models;
using Mango.Web.Models.CouponModels;
using Mango.Web.Models.ShoppingCartModels;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private bool _cartIsEmpty = true;
        public CartController(IProductService productService, ICartService cartService,
            ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, accessToken);
                if (!response.IsSuccess)
                {
                    TempData["Error"] = response.DisplayMessage;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception ex)
            {

                return View(cartDto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            return View();
        }
        public async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            CartDto cartDto = new CartDto
            {
                CartDetails = new List<CartDetailsDto>(),
                CartHeader = new CartHeaderDto()
            };
            if (response != null && response.IsSuccess)
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            if(cartDto != null)
            {
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCouponCode<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);

                    if(coupon != null && coupon.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result));
                        cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                    }
                }
                foreach (var detail in cartDto.CartDetails)
                    cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
            }
            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            return cartDto;
        }
        public async Task<IActionResult> CartRemove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.DeleteCartAsync<ResponseDto>(cartDetailsId,accessToken);

            CartDto cartDto = new();
            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));
            return View();
        }
        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));
            return View();
        }
        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(CartIndex));
            return View();
        }
        
    }
}
