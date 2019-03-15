using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Department
    {
        public Department()
        {
            this.Cells = new List<Cell>();
            this.Officers = new List<Officer>();
        }

        public Department(int id, string name, ICollection<Cell> cells, ICollection<Officer> officers)
        {
            Id = id;
            Name = name;
            Cells = cells;
            Officers = officers;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 25, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<Cell> Cells { get; set; }

        public ICollection<Officer> Officers { get; set; }
    }
}
