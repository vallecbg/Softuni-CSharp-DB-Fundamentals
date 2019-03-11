using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Tag
    {
        public Tag()
        {
        }

        public Tag(int id, string name, ICollection<GameTag> gametags)
        {
            Id = id;
            Name = name;
            GameTags = gametags;
        }

        [Key]

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<GameTag> GameTags { get; set; }
    }
}
