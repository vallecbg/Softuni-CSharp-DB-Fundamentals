using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VaporStore.Data.Models.Enum;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
        }

        public Card(int id, string number, string cvc, CardType type, int userId, User user, ICollection<Purchase> purchases)
        {
            Id = id;
            Number = number;
            Cvc = cvc;
            Type = type;
            UserId = userId;
            User = user;
            Purchases = purchases;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{3}")]
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}
