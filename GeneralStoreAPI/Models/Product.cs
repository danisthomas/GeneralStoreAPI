using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Product
    {
        [Key]
        public string SKU { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int NumberInInventory { get; set; }
        public bool IsInStock
        {
            get
            {
                return NumberInInventory > 1;
            }
        }
    }
}