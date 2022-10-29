namespace Mango.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<T> GetCouponCode<T>(string couponCode, string token = null);
    }
}
