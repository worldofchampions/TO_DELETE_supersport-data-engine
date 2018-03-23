namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;
    using System.Collections.Generic;
    using System.Linq;

    public class LegacyMotorsportDriverStandingsMapperProfile : Profile
    {
        public LegacyMotorsportDriverStandingsMapperProfile()
        {
            CreateMap<MotorsportDriverStandingsEntity, List<DriverStandings>>()

                .ConstructUsing(x => x.MotorsportDriverStandings.Select(y => CreateDestinationObject(x.MotorsportLeague, y)).ToList())

                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<MotorsportDriverStanding, DriverStandings>()

                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.Position))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.Points))

                .ForMember(dest => dest.Wins, expression => expression.MapFrom(
                    src => src.Wins))

                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.MotorsportDriver.LegacyDriverId))

                .ForMember(dest => dest.FirstName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FirstName))

                .ForMember(dest => dest.LastName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.LastName))

                .ForMember(dest => dest.Initials, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FirstName.Substring(0, 1)))

                .ForMember(dest => dest.Abbreviation, expression => expression.UseValue(""))

                .ForMember(dest => dest.FullName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FullNameCmsOverride ?? src.MotorsportDriver.FirstName + " " + src.MotorsportDriver.LastName))

                .ForMember(dest => dest.CarNumber, expression => expression.MapFrom(
                    src => src.MotorsportDriver.CarNumber))

                .ForMember(dest => dest.Country, expression => expression.MapFrom(
                    src => src.MotorsportDriver.CountryName))

                .ForMember(dest => dest.TeamId, expression => expression.MapFrom(
                    src => src.MotorsportTeam.LegacyTeamId))

                .ForMember(dest => dest.TeamName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForMember(dest => dest.TeamShortName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForAllOtherMembers(dest => dest.Ignore());
        }

        private static DriverStandings CreateDestinationObject(MotorsportLeague motorsportLeague, MotorsportDriverStanding motorsportDriverStanding)
        {
            var destination = Mapper.Map<MotorsportDriverStanding, DriverStandings>(motorsportDriverStanding);

            destination.LeagueName = motorsportLeague.NameCmsOverride ?? motorsportLeague.Name;
            destination.LeagueURLName = motorsportLeague.Slug;

            return destination;
        }
    }
}
