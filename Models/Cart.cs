using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TruYumClientApplication.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
