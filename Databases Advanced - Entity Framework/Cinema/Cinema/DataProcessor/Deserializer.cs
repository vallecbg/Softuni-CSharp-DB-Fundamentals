using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Cinema.Data.Models;
using Cinema.Data.Models.Enums;
using Cinema.DataProcessor.ImportDto;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var resultMessages = new List<string>();

            var movies = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);
            var resultMovies = new List<Movie>();

            foreach (var movieDto in movies)
            {
                if (movieDto.Title == null || movieDto.Title.Length < 3 || movieDto.Title.Length > 20 ||
                    movieDto.Genre == null || 
                    movieDto.Duration == null ||
                    movieDto.Rating == null || movieDto.Rating < 1 || movieDto.Rating > 10 ||
                    movieDto.Director == null || movieDto.Director.Length < 3 || movieDto.Director.Length > 20)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                //todo check
                if (!IsValid(movieDto))
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                bool alreadyExists = resultMovies.Any(m => m.Title == movieDto.Title);

                if (alreadyExists == true)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                Movie movie = new Movie()
                {
                    Title = movieDto.Title,
                    Genre = Enum.Parse<Genre>(movieDto.Genre),
                    Duration = TimeSpan.Parse(movieDto.Duration),
                    Rating = movieDto.Rating,
                    Director = movieDto.Director
                };

                resultMovies.Add(movie);
                resultMessages.Add(string.Format(SuccessfulImportMovie, movieDto.Title, movieDto.Genre, $"{movieDto.Rating:f2}"));
            }

            context.Movies.AddRange(resultMovies);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var resultMessages = new List<string>();

            var hallSeats = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);
            var resultHallSeats = new List<Hall>();

            foreach (var hallSeatDto in hallSeats)
            {
                if (hallSeatDto.Name == null || hallSeatDto.Name.Length < 3 || hallSeatDto.Name.Length > 20 ||
                    hallSeatDto.Is3D == null ||
                    hallSeatDto.Is4Dx == null || 
                    hallSeatDto.Seats <= 0)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                if (!IsValid(hallSeatDto))
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                Hall hall = new Hall()
                {
                    Name = hallSeatDto.Name,
                    Is4Dx = hallSeatDto.Is4Dx,
                    Is3D = hallSeatDto.Is3D
                };
                var hallSeatsResult = new List<Seat>();
                for (int i = 1; i <= hallSeatDto.Seats; i++)
                {
                    Seat seat = new Seat()
                    {
                        Hall = hall
                    };
                    hallSeatsResult.Add(seat);
                }

                hall.Seats = hallSeatsResult;

                resultHallSeats.Add(hall);
                string projectionType;
                if (hall.Is3D && hall.Is4Dx)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hall.Is4Dx)
                {
                    projectionType = "4Dx";
                }
                else if (hall.Is3D)
                {
                    projectionType = "3D";
                }
                else
                {
                    projectionType = "Normal";
                }
                resultMessages.Add(string.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hallSeatDto.Seats));
                resultHallSeats.Add(hall);
            }

            context.Halls.AddRange(resultHallSeats);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var resultMessages = new List<string>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var resultProjections = new List<Projection>();

            foreach (var projectionDto in projectionsDto)
            {
                if (projectionDto.MovieId == null || projectionDto.MovieId == 0 ||
                    projectionDto.HallId == null ||
                    projectionDto.HallId == 0 ||
                    projectionDto.DateTime == null ||
                    projectionDto.DateTime.Length < 18)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                if (!IsValid(projectionDto))
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                Movie movie = context.Movies.FirstOrDefault(x => x.Id == projectionDto.MovieId);
                Hall hall = context.Halls.FirstOrDefault(x => x.Id == projectionDto.HallId);
                if (movie == null || hall == null)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                Projection projection = new Projection()
                {
                    MovieId = projectionDto.MovieId,
                    Movie = movie,
                    HallId = projectionDto.HallId,
                    Hall = hall,
                    DateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                resultProjections.Add(projection);

                resultMessages.Add(string.Format(SuccessfulImportProjection, movie.Title, projection.DateTime.ToString("MM/dd/yyyy")));
            }

            context.Projections.AddRange(resultProjections);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }
        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var resultMessages = new List<string>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketsDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerTicketsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var resultCustomers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                if (customerDto.FirstName == null || customerDto.FirstName.Length < 3 || customerDto.FirstName.Length > 20 ||
                    customerDto.LastName == null ||
                    customerDto.LastName.Length < 3 ||
                    customerDto.LastName.Length > 20 ||
                    customerDto.Age == null || customerDto.Age < 12 || customerDto.Age > 110 ||
                    customerDto.Balance == null || customerDto.Balance < 0.01m)
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                if (!IsValid(customerDto))
                {
                    resultMessages.Add(ErrorMessage);
                    continue;
                }

                Customer customer = new Customer()
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance
                };

                var customerTickets = new List<Ticket>();
                foreach (var ticketDto in customerDto.Tickets)
                {
                    Ticket ticket = new Ticket()
                    {
                        ProjectionId = ticketDto.ProjectionId,
                        Customer = customer,
                        Price = ticketDto.Price
                    };

                    customerTickets.Add(ticket);
                }

                customer.Tickets = customerTickets;

                resultCustomers.Add(customer);
                resultMessages.Add(String.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }

            context.Customers.AddRange(resultCustomers);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}