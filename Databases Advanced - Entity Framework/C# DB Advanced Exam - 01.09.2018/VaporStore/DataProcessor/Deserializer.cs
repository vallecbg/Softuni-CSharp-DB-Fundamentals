using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enum;
using VaporStore.DataProcessor.Dto;

namespace VaporStore.DataProcessor
{
	using System;
	using Data;

	public static class Deserializer
    {
        public const string InvalidMessage = "Invalid Data";
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            var resultMessages = new List<string>();

            var games = JsonConvert.DeserializeObject<ImportGamesDto[]>(jsonString);
            var resultGames = new List<Game>();

            foreach (var gameDto in games)
            {
                if (gameDto.Price < 0.00m ||
                    gameDto.Name == null || gameDto.ReleaseDate == null || gameDto.Developer == null || gameDto.Genre == null || gameDto.Tags == null || gameDto.Tags.Count == 0)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                var currGame = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                };

                var developer = GetDeveloper(context, gameDto.Developer);
                var genre = GetGenre(context, gameDto.Genre);

                currGame.Developer = developer;
                currGame.Genre = genre;
                foreach (var gameDtoTag in gameDto.Tags)
                {
                    var tag = GetTag(context, gameDtoTag);
                    currGame.GameTags.Add(new GameTag()
                    {
                        Game = currGame,
                        Tag = tag
                    });
                }
                resultMessages.Add($"Added {currGame.Name} ({currGame.Genre.Name}) with {currGame.GameTags.Count} tags");
                resultGames.Add(currGame);
            }

            context.Games.AddRange(resultGames);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        private static Tag GetTag(VaporStoreDbContext context, string gameTag)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Name == gameTag);
            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = gameTag
                };
                
                context.Tags.Add(tag);
                context.SaveChanges();
            }

            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string gameGenre)
        {
            var genre = context.Genres.FirstOrDefault(x => x.Name == gameGenre);
            if (genre == null)
            {
                genre = new Genre()
                {
                    Name = gameGenre
                };

                context.Genres.Add(genre);
                context.SaveChanges();
            }

            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string developerName)
        {
            var developer = context.Developers.FirstOrDefault(x => x.Name == developerName);
            if (developer == null)
            {
                developer = new Developer()
                {
                    Name = developerName
                };

                context.Developers.Add(developer);
                context.SaveChanges();
            }

            return developer;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
            var resultMessages = new List<string>();

            var usersDto = JsonConvert.DeserializeObject<ImportUsersDto[]>(jsonString);
            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                if (!Regex.Match(userDto.FullName, @"[A-Z][a-z]+ [A-Z][a-z]+").Success ||
                    userDto.FullName == "" ||
                    userDto.Username.Length < 3 || userDto.Username.Length > 20 ||
                    userDto.Email == null || userDto.Email == "" ||
                    userDto.Age < 3 || userDto.Age > 103 ||
                    userDto.Cards == null || userDto.Cards.Count == 0)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                var user = new User()
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                foreach (var userDtoCard in userDto.Cards)
                {
                    var cardTypeParse = Enum.TryParse<CardType>(userDtoCard.Type, out CardType cardType);
                    if (!Regex.Match(userDtoCard.Number, @"[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}").Success ||
                        !Regex.Match(userDtoCard.Cvc, @"[0-9]{3}").Success ||
                        cardTypeParse == false ||
                        cardType == null)
                    {
                        resultMessages.Add(InvalidMessage);
                        continue;
                    }
                    Card card = new Card()
                    {
                        Number = userDtoCard.Number,
                        Cvc = userDtoCard.Cvc,
                        Type = cardType
                    };

                    user.Cards.Add(card);
                }

                users.Add(user);
                resultMessages.Add($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
            var messagesOutput = new List<string>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchasesDto[]), new XmlRootAttribute("Purchases"));

            var purchasesDto = (ImportPurchasesDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var purchases = new List<Purchase>();

            foreach (var purchaseDto in purchasesDto)
            {
                var purchaseTypeSuccessful = Enum.TryParse<PurchaseType>(purchaseDto.Type, out PurchaseType purchaseType);

                if (!purchaseTypeSuccessful || purchaseType == null ||
                    !Regex.Match(purchaseDto.Key, "[A-Z0-9]+-[A-Z0-9]+-[A-Z0-9]+").Success ||
                    !Regex.Match(purchaseDto.Card, @"[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}").Success ||
                    purchaseDto.Date.Length < 10)
                {
                    messagesOutput.Add(InvalidMessage);
                    continue;
                }

                var card = context.Cards.FirstOrDefault(x => x.Number == purchaseDto.Card);
                var user = context.Users.FirstOrDefault(x => x.Id == card.UserId);
                if (card == null || user == null)
                {
                    messagesOutput.Add(InvalidMessage);
                    continue;
                }

                var date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var game = context.Games.FirstOrDefault(x => x.Name == purchaseDto.Title);

                var purchase = new Purchase()
                {
                    Type = purchaseType,
                    ProductKey = purchaseDto.Key,
                    Card = card,
                    Date = date,
                    Game = game
                };

                

                purchases.Add(purchase);
                messagesOutput.Add($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return string.Join(Environment.NewLine, messagesOutput);
        }
	}
}