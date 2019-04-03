using System.Linq;
using AutoMapper;
using FastFood.DataProcessor.Dto.Export;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;

namespace FastFood.App
{
	public class FastFoodProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public FastFoodProfile()
        {
            CreateMap<ImportOrdersDto, Order>();
            CreateMap<ItemDto, Item>();

            //XmlExport
            //CreateMap<Category, ExportCategoryStatisticsDto>();
            //CreateMap<Item, MostPopularItemDto>();

            //CreateMap<OrderItem, MostPopularItemDto>()
            //    .ForMember(x => x.TotalMade, y => y.MapFrom(c => c.Quantity * c.Item.Price))
            //    .ForMember(x => x.TimesSold, y => y.MapFrom(c => c.Quantity));
        }
	}
}
