﻿using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.RabbitMQSender;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [Authorize]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageBus _messageBus;
        protected ResponseDto _response;
        private ICouponRepository _couponRepository;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
        public CartAPIController(ICartRepository cartRepository, 
                                    IMessageBus messageBus, 
                                    ICouponRepository couponRepository,
                                    IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _response = new ResponseDto();
            _couponRepository = couponRepository;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
        }
        [HttpGet("GetCard/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("UpdateCart")]
        public async Task<object> UdpateCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                bool success = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = success;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool success = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, 
                                                                    cartDto.CartHeader.CouponCode);
                _response.Result = success;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool success = await _cartRepository.RemoveCoupon(userId);
                _response.Result = success;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("ClearCart/{userId}")]
        public async Task<object> ClearCart(string userId)
        {
            try
            {
                bool success = await _cartRepository.ClearCart(userId);
                _response.Result = success;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if (cartDto == null)
                    return BadRequest();

                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if(checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon price has changed, please confirm" };
                        _response.DisplayMessage = "Coupon price has changed, please confirm";
                        return _response;
                    }
                }

                checkoutHeader.CartDetails = cartDto.CartDetails;
                // logic to add message to proccess order.
                // await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");
                // await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                // RabbitMQ
                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");
                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
    }   
}
