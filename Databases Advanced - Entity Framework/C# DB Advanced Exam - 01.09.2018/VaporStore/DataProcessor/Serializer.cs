using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enum;
using VaporStore.DataProcessor.Dto.Export;
using Formatting = Newtonsoft.Json.Formatting;

namespace VaporStore.DataProcessor
{
    using System;
    using Data;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var games = context
                .Genres
                .Where(x => genreNames.Contains(x.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games
                        .Where(p => p.Purchases.Any())
                        .Select(g => new
                        {
                            Id = g.Id,
                            Title = g.Name,
                            Developer = g.Developer.Name,
                            //check
                            Tags = string.Join(", ", g.GameTags.Select(c => c.Tag.Name)),
                            Players = g.Purchases.Count
                        }).OrderByDescending(c => c.Players)
                        .ThenBy(c => c.Id),
                    TotalPlayers = x.Games.Sum(c => c.Purchases.Count)
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id);

            var json = JsonConvert.SerializeObject(games, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var purchases = context
                .Purchases
                .Where(x => x.Type.ToString() == storeType)
                .Select(x => new PurchasesDto
                {
                    Card = x.Card.Number,
                    Cvc = x.Card.Cvc,
                    Date = x.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Game = new GameDto()
                    {
                        Title = x.Game.Name,
                        Genre = x.Game.Genre.Name,
                        Price = x.Game.Price
                    }
                })
                .ToArray();

            var users = context
                    .Users
                    .Select(x => new ExportUserPurchasesByType()
                    {
                        UserName = x.Username,
                        Purchases = x.Cards
                            .SelectMany(p => p.Purchases)
                            .Where(c => c.Type == purchaseType)
                            .Select(p => new PurchasesDto()
                            {
                                Card = p.Card.Number,
                                Cvc = p.Card.Cvc,
                                Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                                Game = new GameDto
                                {
                                    Genre = p.Game.Genre.Name,
                                    Title = p.Game.Name,
                                    Price = p.Game.Price
                                }
                            })
                            .OrderBy(d => d.Date)
                            .ToArray(),
                        TotalSpent = x.Cards.SelectMany(p => p.Purchases)
                            .Where(t => t.Type == purchaseType)
                            .Sum(p => p.Game.Price)
                    })
                    .Where(p => p.Purchases.Any())
                    .OrderByDescending(t => t.TotalSpent)
                    .ThenBy(u => u.UserName)
                    .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportUserPurchasesByType[]), new XmlRootAttribute("Users"));
            serializer.Serialize(new StringWriter(sb), users, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
        }
    }
}