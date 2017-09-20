using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyFixtureMapperProfile : Profile
    {
        public LegacyFixtureMapperProfile()
        {
            CreateMap<RugbyResult, FixtureModel>()

                //AWAY TEAM
                .ForMember(rf => rf.AwayTeam, fm => fm.MapFrom(r => r.Fixture.AwayTeam.Name))
                .ForMember(rf => rf.AwayTeamScore, fm => fm.MapFrom(f => f.AwayTeamScore))
                .ForMember(rf => rf.AwayTeamId, fm => fm.MapFrom(f => f.Fixture.AwayTeam.ProviderTeamId))
                .ForMember(rf => rf.AwayTeamPenalties, fm => fm.UseValue(0))
                .ForMember(rf => rf.AwayTeamShortName, fm => fm.MapFrom(f => f.Fixture.AwayTeam.Abbreviation))

                //HOME TEAM
                .ForMember(rf => rf.HomeTeam, fm => fm.MapFrom(r => r.Fixture.HomeTeam.Name))
                .ForMember(rf => rf.HomeTeamScore, fm => fm.MapFrom(f => f.HomeTeamScore))
                .ForMember(rf => rf.HomeTeamId, fm => fm.MapFrom(f => f.Fixture.HomeTeam.ProviderTeamId))
                .ForMember(rf => rf.HomeTeamPenalties, fm => fm.UseValue(0))
                .ForMember(rf => rf.HomeTeamShortName, fm => fm.MapFrom(f => f.Fixture.HomeTeam.Abbreviation))

                //FIXTURE
                .ForMember(rf => rf.channelRegions, fm => fm.Ignore())
                .ForMember(rf => rf.Channels, fm => fm.Ignore())
                .ForMember(rf => rf.International, fm => fm.UseValue(false))
                .ForMember(rf => rf.IsFeatured, fm => fm.UseValue(false))
                .ForMember(rf => rf.LeagueId, fm => fm.MapFrom(f => f.Fixture.RugbyTournament.LegacyTournamentId))
                .ForMember(rf => rf.LeagueName, fm => fm.MapFrom(f => f.Fixture.RugbyTournament.Name))
                .ForMember(rf => rf.LeagueShortName, fm => fm.MapFrom(f => f.Fixture.RugbyTournament.Abbreviation))
                .ForMember(rf => rf.LeagueUrlName, fm => fm.MapFrom(f => f.Fixture.RugbyTournament.LogoUrl))
                .ForMember(rf => rf.Location, fm => fm.MapFrom(f => f.Fixture.RugbyVenue))
                .ForMember(rf => rf.MatchDateTime, fm => fm.MapFrom(f => f.Fixture.StartDateTime.UtcDateTime))
                .ForMember(rf => rf.MatchDateTimeString, fm => fm.MapFrom(f => f.Fixture.StartDateTime.UtcDateTime.ToLongDateString()))
                .ForMember(rf => rf.MatchID, fm => fm.MapFrom(f => f.Fixture.LegacyFixtureId))
                .ForMember(rf => rf.Result, fm => fm.MapFrom(f => f.Fixture.RugbyFixtureStatus == RugbyFixtureStatus.GameEnd))
                .ForMember(rf => rf.Sport, fm => fm.UseValue(SportType.Rugby))
                .ForMember(rf => rf.WalkOver, fm => fm.UseValue(false))

                //NOISY DATA
                .ForAllOtherMembers(rf => rf.Ignore());
        }
    }
}