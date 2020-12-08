using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TruYumClientApplication.Models
{
    public class MenuItem
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
    }
}
