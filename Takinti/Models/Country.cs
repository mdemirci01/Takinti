using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Takinti.Models
{
    public class Country
    {
        public int Id { get; set; }
        [StringLength(200)]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}