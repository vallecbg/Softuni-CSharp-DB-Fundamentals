using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FastFood.Models
{
    public class Position
    {
        public Position()
        {
            this.Employees = new HashSet<Employee>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        //TODO Add isUnique
        [StringLength(maximumLength: 30, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
