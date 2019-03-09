using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Developer
    {
        public Developer()
        {
        }

        public Developer(int id, string name, ICollection<Game> games)
        {
            this.Id = id;
            this.Name = name;
            this.Games = games;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
