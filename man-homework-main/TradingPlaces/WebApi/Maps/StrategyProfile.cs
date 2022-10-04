using AutoMapper;
using Models.Strategy;
using TradingPlaces.WebApi.Dtos;

namespace TradingPlaces.WebApi.Maps
{
    public class StrategyProfile : Profile
    {
        public StrategyProfile()
        {
            CreateMap<Strategy, StrategyDetailsDto>();
        }
    }
}