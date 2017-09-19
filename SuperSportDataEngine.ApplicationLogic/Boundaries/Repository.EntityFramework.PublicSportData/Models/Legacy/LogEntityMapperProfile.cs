using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Entities;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Legacy
{
    public class LogEntityMapperProfile : Profile
    {
        public LogEntityMapperProfile()
        {
            // TODO: This was commented out when deleting old Log DB table.
            //CreateMap<Log, LogEntity>();
        }
    }
}