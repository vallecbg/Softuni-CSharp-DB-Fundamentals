using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Cell
    {
        public Cell()
        {
            this.Prisoners = new List<Prisoner>();
        }

        public Cell(int id, int cellNumber, bool hasWindow, int departmentId, Department department, ICollection<Prisoner> prisoners)
        {
            Id = id;
            CellNumber = cellNumber;
            HasWindow = hasWindow;
            DepartmentId = departmentId;
            Department = department;
            Prisoners = prisoners;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }

        [Required]
        public bool HasWindow { get; set; }

        //[ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        //[Required]
        public Department Department { get; set; }

        public ICollection<Prisoner> Prisoners { get; set; }
    }
}
