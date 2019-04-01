using System.Linq;
using AutoMapper;
using CarDealer.Dtos.Export;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //16
            CreateMap<Supplier, GetLocalSuppliersDto>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(c => c.Parts.Count));

            //17
            CreateMap<Car, GetCarsWithTheirListOfPartsDto>()
                .ForMember(x => x.Parts, y => y.MapFrom(c => c.PartCars
                    .Select(z => z.Part)
                    .OrderByDescending(v => v.Price)));

            CreateMap<Part, GetPartsDto>();

            //18
            CreateMap<Customer, GetTotalSalesByCustomerDto>()
                .ForMember(x => x.FullName, y => y.MapFrom(c => c.Name))
                .ForMember(x => x.BoughtCars, y => y.MapFrom(c => c.Sales.Select(z => z.Car).Count()))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(
                    c => c.Sales.Sum(z => z.Car.PartCars.Sum(pc => pc.Part.Price))));

            //19
            CreateMap<Sale, GetSalesWithAppliedDiscountDto>()
                .ForMember(x => x.CustomerName, y => y.MapFrom(c => c.Customer.Name))
                //i think its not necessary
                .ForMember(x => x.Car, y => y.MapFrom(c => c.Car))
                .ForMember(x => x.Price, y => y.MapFrom(c => c.Car.PartCars.Sum(pc => pc.Part.Price)))
                .ForMember(x => x.PriceWithDiscount, y => y.MapFrom(
                    obj => $"{obj.Car.PartCars.Sum(z => z.Part.Price) - (obj.Car.PartCars.Sum(w => w.Part.Price) * (obj.Discount / 100))}".TrimEnd('0')));


            CreateMap<Car, CarDto>()
                .ForMember(x => x.Make, y => y.MapFrom(c => c.Make))
                .ForMember(x => x.Model, y => y.MapFrom(c => c.Model))
                .ForMember(x => x.TravelledDistance, y => y.MapFrom(c => c.TravelledDistance));
        }
    }
}
