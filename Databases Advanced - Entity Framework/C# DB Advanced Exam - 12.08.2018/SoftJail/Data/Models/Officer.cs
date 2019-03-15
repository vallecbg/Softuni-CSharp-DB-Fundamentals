using SoftJail.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Officer
    {
        public Officer()
        {
            this.OfficerPrisoners = new List<OfficerPrisoner>();
        }

        public Officer(int id, string fullName, decimal salary, Position position, Weapon weapon, int departmentId, Department department, ICollection<OfficerPrisoner> officerPrisoners)
        {
            Id = id;
            FullName = fullName;
            Salary = salary;
            Position = position;
            Weapon = weapon;
            DepartmentId = departmentId;
            Department = department;
            OfficerPrisoners = officerPrisoners;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        public Weapon Weapon { get; set; }

        //[ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        //[Required]
        public Department Department { get; set; }

        public ICollection<OfficerPrisoner> OfficerPrisoners { get; set; }
    }
}
