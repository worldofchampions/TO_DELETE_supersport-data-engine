namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyMotorsportTeamStandingsMapperProfile : Profile
    {
        public LegacyMotorsportTeamStandingsMapperProfile()
        {
            CreateMap<MotorsportTeamStanding, TeamStandings>()

                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.Position))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.Points))

                .ForMember(dest => dest.TeamId, expression => expression.MapFrom(
                    src => src.MotorsportTeam.LegacyTeamId))

                .ForMember(dest => dest.TeamName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForMember(dest => dest.TeamShortName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
