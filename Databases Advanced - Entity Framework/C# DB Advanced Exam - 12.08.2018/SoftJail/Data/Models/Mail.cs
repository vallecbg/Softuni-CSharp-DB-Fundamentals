﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        public Mail()
        {
        }

        public Mail(int id, string description, string sender, string address, int prisonerId, Prisoner prisoner)
        {
            Id = id;
            Description = description;
            Sender = sender;
            Address = address;
            PrisonerId = prisonerId;
            Prisoner = prisoner;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9\s]*(str\.)")]
        public string Address { get; set; }

        //[ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }
        //[Required]
        public Prisoner Prisoner { get; set; }
    }
}
