using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.ShoppingCartModels
{
    public class CartHeaderDto
    {
        private DateTime _releaseDate = DateTime.MinValue;
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}")]
        public DateTime PickupDateTime 
        {
            get
            {
                return (_releaseDate == DateTime.MinValue) ? DateTime.Now : _releaseDate;
            }
            set
            {
                _releaseDate = value;
            } 
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpireMonthlyYear { get; set; }
    }
}
