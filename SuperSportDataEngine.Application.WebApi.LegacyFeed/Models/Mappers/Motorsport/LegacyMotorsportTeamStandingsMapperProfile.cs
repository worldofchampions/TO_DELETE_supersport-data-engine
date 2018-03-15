namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;
    using System.Collections.Generic;
    using System.Linq;

    public class LegacyMotorsportTeamStandingsMapperProfile : Profile
    {
        public LegacyMotorsportTeamStandingsMapperProfile()
        {
            CreateMap<MotorsportTeamStandingsEntity, List<TeamStandings>>()

                .ConstructUsing(x => x.MotorsportTeamStandings.Select(y => CreateFeedObject(y, x.MotorsportLeague)).ToList())

                .ForAllOtherMembers(dest => dest.Ignore());

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

        private static TeamStandings CreateFeedObject(MotorsportTeamStanding motorsportTeamStanding, MotorsportLeague motorsportLeague)
        {
            var destination = Mapper.Map<MotorsportTeamStanding, TeamStandings>(motorsportTeamStanding);

            destination.LeagueName = motorsportLeague.NameCmsOverride ?? motorsportLeague.Name;
            destination.LeagueURLName = motorsportLeague.Slug;

            return destination;
        }
    }
}
