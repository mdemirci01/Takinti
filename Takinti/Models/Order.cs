using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Takinti.Models
{
    public class Order
    {
        public Order()
        {
            OrderDate = DateTime.Now;
            OrderItems = new HashSet<OrderItem>();
        }
        public int Id { get; set; }
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }
        public DateTime OrderDate { get; set; }
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int CountryId { get; set; }
        public string CompanyName { get; set; }
        public int CityId { get; set; }
        public string PostalCode { get; set; }
        public string ShippingNumber { get; set; }
        // kredi kartı bilgileri
        public string CardHolderName { get; set; }
        public string CardNumberLastFourDigit { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public string UserName { get; set; }
    }
}