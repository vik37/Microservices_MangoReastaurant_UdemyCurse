using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            // check if product exist in db, if not create it!
            var productInDb = await _db.Product
                                        .FirstOrDefaultAsync(p => p.Id == cartDto.CartDetails.FirstOrDefault()
                                                        .ProductId);
            if(productInDb == null)
            {
                _db.Product.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }
            // Check if Header is null!
            var cartHeaderfromDb = await _db.CartHeader
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(ch => ch.UserId == cart.CartHeader.UserId);
            if(cartHeaderfromDb == null)
            {
                // Create Header and Details
                _db.CartHeader.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();
            }
            else
            {
                // If cart hheader is not null;
                // Check if details has same product
                var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    cd => cd.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId &&
                    cd.CartHeaderId == cartHeaderfromDb.CartHeaderId);

                if(cartDetailsFromDb == null)
                {
                    // Create Details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderfromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Update the count or cart details
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _db.CartHeader.FirstOrDefaultAsync(ch => ch.UserId == userId)
            };
            cart.CartDetails = _db.CartDetails
                                .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId)
                                .Include(cd => cd.Product);
            return _mapper.Map<CartDto>(cart);
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _db.CartHeader.FirstOrDefaultAsync(ch => ch.UserId == userId);
            if(cartHeaderFromDb != null)
            {
                _db.CartDetails.RemoveRange(_db.CartDetails.Where(cd => cd.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _db.CartHeader.Remove(cartHeaderFromDb);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }       
        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails
                                                .FirstOrDefaultAsync(cd => cd.CartDetailsId == cartDetailId);
                int totalCountOfCartItems = _db.CartDetails
                                            .Where(cd => cd.CartHeaderId == cartDetails.CartHeaderId)
                                            .Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeader
                                                    .FirstOrDefaultAsync(ch => ch.CartHeaderId == cartDetails.CartHeaderId);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartFromDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = couponCode;
            _db.CartHeader.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartFromDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = "";
            _db.CartHeader.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
