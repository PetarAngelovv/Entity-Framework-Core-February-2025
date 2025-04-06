using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportCarDto, Car>()
            .ForMember(dest => dest.PartsCars, opt => opt.Ignore()); // Защото ги добавяш ръчно
        }
    }
}
