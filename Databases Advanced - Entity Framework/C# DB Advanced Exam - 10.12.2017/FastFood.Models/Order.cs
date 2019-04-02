using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using FastFood.Models.Enums;

namespace FastFood.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required] public OrderType Type { get; set; } = OrderType.ForHere;

        [Required] [NotMapped] public decimal TotalPrice => this.OrderItems.Sum(x => x.Item.Price * x.Quantity); 

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
