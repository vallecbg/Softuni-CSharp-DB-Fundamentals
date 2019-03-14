using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App
{
    public class Database
    {
        public static void Seed(BillsPaymentSystemContext db)
        {
            using (db)
            {
                var user = new User
                {
                    FirstName = "Guy",
                    LastName = "Gilbert",
                    Email = "Gilbert@abv.bg",
                    Password = "HashPassword"
                };

                var card = new CreditCard
                {
                    ExpirationDate = new DateTime(2020, 03, 1),
                    Limit = 800m,
                    MoneyOwed = 100m
                };

                var bankAccounts = new[]
                {
                new BankAccount
                {
                    Balance=2000m,
                    BankName="Unicredit Bulbank",
                    SwiftCode="UNCRBGSF"
                },
                new BankAccount
                {
                    Balance=1000m,
                    BankName="First Investment Bank",
                    SwiftCode="FINVBGSF"
                }
            }
                    .ToList();

                var paymentMethods = new[]
                    {
                new PaymentMethod
                {
                    User=user,
                    BankAccount=bankAccounts[0],
                    Type=Models.Enums.Type.BankAccount
                },
                 new PaymentMethod
                {
                    User=user,
                    BankAccount=bankAccounts[1],
                    Type=Models.Enums.Type.BankAccount
                },
                new PaymentMethod
                {
                    User=user,
                    CreditCard=card,
                    Type=Models.Enums.Type.CreditCard
                }
            }
                    .ToList();

                db.Users.Add(user);
                db.CreditCards.Add(card);
                db.BankAccounts.AddRange(bankAccounts);
                db.PaymentMethods.AddRange(paymentMethods);
                db.SaveChanges();
            }
        }

        public static void UserDetails(int userId, BillsPaymentSystemContext db)
        {
            using (db = new BillsPaymentSystemContext())
            {
                var user = db.Users
                .Where(x => x.UserId == userId)
                .Select(x => new
                {
                    Name = $"{x.FirstName} {x.LastName}",
                    BankAccounts = x.PaymentMethod
                        .Where(a => a.Type == Models.Enums.Type.BankAccount)
                        .Select(b => b.BankAccount).ToList(),
                    CreditCards = x.PaymentMethod
                        .Where(a => a.Type == Models.Enums.Type.CreditCard)
                        .Select(n => n.CreditCard).ToList()
                }).FirstOrDefault();

                if (user == null)
                {
                    Console.WriteLine("There is no user with that Id");
                    return;
                }
                Console.WriteLine($"User: {user.Name}");

                if (user.BankAccounts.Any())
                {
                    Console.WriteLine("Bank Accounts:");
                    foreach (var item in user.BankAccounts)
                    {
                        Console.WriteLine($"-- ID: {item.BankAccountId}");
                        Console.WriteLine($"--- Balance: {item.Balance:f2}");
                        Console.WriteLine($"--- Bank: {item.BankName}");
                        Console.WriteLine($"--- SWIFT: {item.SwiftCode}");
                    }
                }

                if (!user.CreditCards.Any()) return;
                {
                    Console.WriteLine("Credit Cards:");
                    foreach (var item in user.CreditCards)
                    {
                        Console.WriteLine($"-- ID: {item.CreditCardId}");
                        Console.WriteLine($"--- Limit: {item.Limit:f2}");
                        Console.WriteLine($"--- Money Owed: {item.MoneyOwed:f2}");
                        Console.WriteLine($"--- Limit Left: {item.LimitLeft}");
                        Console.WriteLine($"--- Expiration Date: {item.ExpirationDate.ToString(@"yyyy/MM", CultureInfo.InvariantCulture)}");
                    }
                }
            }

        }
    }
}
