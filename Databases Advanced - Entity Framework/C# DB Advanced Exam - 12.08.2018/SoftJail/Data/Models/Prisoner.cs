using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        public Prisoner()
        {
            this.Mails = new List<Mail>();
            this.PrisonerOfficers = new List<OfficerPrisoner>();
        }

        public Prisoner(int id, string fullName, string nickname, int age, DateTime incarcerationDate, DateTime releaseDate, decimal bail, int cellId, Cell cell, ICollection<Mail> mails, ICollection<OfficerPrisoner> prisonerOfficers)
        {
            Id = id;
            FullName = fullName;
            Nickname = nickname;
            Age = age;
            IncarcerationDate = incarcerationDate;
            ReleaseDate = releaseDate;
            Bail = bail;
            CellId = cellId;
            Cell = cell;
            Mails = mails;
            PrisonerOfficers = prisonerOfficers;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"The [A-Z]{1}[a-z]*")]
        public string Nickname { get; set; }

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }
        
        public DateTime? ReleaseDate { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal? Bail { get; set; }

        //[ForeignKey(nameof(Cell))]
        public int CellId { get; set; }
        public Cell Cell { get; set; }

        public ICollection<Mail> Mails { get; set; }

        public ICollection<OfficerPrisoner> PrisonerOfficers { get; set; }
    }
}