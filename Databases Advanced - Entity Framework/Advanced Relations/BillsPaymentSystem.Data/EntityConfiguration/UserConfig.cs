using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfiguration
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(f => f.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.Email)
                .IsUnicode(false)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(f => f.Password)
                .IsUnicode(false)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}
