using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Takinti.Models
{
    public class CheckoutViewModel
    {
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int CountryId { get; set; }
        public string CompanyName { get; set; }
        public int CityId { get; set; }
        public string PostalCode { get; set; }
        // kredi kartı bilgileri
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CCV { get; set; }
    }
}