using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Entities;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LogModelMapperProfile : Profile
    {
        public LogModelMapperProfile()
        {
            CreateMap<LogEntity, LogModel>();
        }
    }
}