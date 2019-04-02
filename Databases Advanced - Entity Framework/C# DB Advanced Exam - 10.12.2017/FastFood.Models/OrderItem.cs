using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;

namespace FastFood.Models
{
    public class OrderItem
    {
        //TODO Add primary key orderid itemid
        public int OrderId { get; set; }
        [Required]
        public Order Order { get; set; }

        public int ItemId { get; set; }
        [Required]
        public Item Item { get; set; }

        [Required]
        [Range(1, 2147483647)]
        public int Quantity { get; set; }
    }
}
