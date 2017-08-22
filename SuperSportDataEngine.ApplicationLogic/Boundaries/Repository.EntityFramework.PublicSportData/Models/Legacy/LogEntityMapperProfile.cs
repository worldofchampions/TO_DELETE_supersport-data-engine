using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Entities;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Legacy
{
    public class LogEntityMapperProfile : Profile
    {
        public LogEntityMapperProfile()
        {
            CreateMap<Log, LogEntity>();
        }
    }
}