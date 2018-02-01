namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyGroupedLogMapperProfile : Profile
    {
        public LegacyGroupedLogMapperProfile()
        {
            CreateMap<RugbyGroupedLog, Log>()

                .ForMember(dest => dest.GroupName, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.GroupName))

                .ForMember(dest => dest.GroupShortName, expression => expression.MapFrom(
                   src => src.RugbyLogGroup.GroupShortName ?? src.RugbyLogGroup.GroupName))

                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(
                    src => src.RugbyTournament.NameCmsOverride ?? src.RugbyTournament.Name))

                .ForMember(dest => dest.Team, expression => expression.MapFrom(
                    src => (src.RugbyTeam.NameCmsOverride ?? src.RugbyTeam.Name)))

                .ForMember(dest => dest.TeamID, expression => expression.MapFrom(
                    src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.LogPosition))

                .ForMember(dest => dest.Played, expression => expression.MapFrom(
                    src => src.GamesPlayed))

                .ForMember(dest => dest.Won, expression => expression.MapFrom(
                    src => src.GamesWon))

                .ForMember(dest => dest.Drew, expression => expression.MapFrom(
                    src => src.GamesDrawn))

                .ForMember(dest => dest.Lost, expression => expression.MapFrom(
                    src => src.GamesLost))

                .ForMember(dest => dest.PointsFor, expression => expression.MapFrom(
                    src => src.PointsFor))

                .ForMember(dest => dest.PointsAgainst, expression => expression.MapFrom(
                    src => src.PointsAgainst))

                .ForMember(dest => dest.BonusPoints, expression => expression.MapFrom(
                    src => src.BonusPoints))

                .ForMember(dest => dest.PointsDifference, expression => expression.MapFrom(
                    src => src.PointsDifference))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.TournamentPoints))

                .ForMember(dest => dest.Sport, src => src.UseValue(SportType.Rugby))

                // rank here
                .ForMember(dest => dest.rank, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.GroupHierarchyLevel == LegacyFeedConstants.GroupHiearachyLevelZero ?
                    src.LogPosition : 0))

                // Combined rank here
                .ForMember(dest => dest.CombinedRank, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.GroupHierarchyLevel == LegacyFeedConstants.GroupHiearachyLevelOne ?
                    src.LogPosition : 0))

                 // ConferenceRank rank here
                 .ForMember(dest => dest.ConferenceRank, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.GroupHierarchyLevel == LegacyFeedConstants.GroupHiearachyLevelTwo ?
                    src.LogPosition : 0))

                // Home group here
                .ForMember(dest => dest.HomeGroup, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.ParentRugbyLogGroup != null ?
                    src.RugbyLogGroup.ParentRugbyLogGroup.GroupName : src.RugbyLogGroup.GroupName))

                .ForMember(dest => dest.IsConference, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.IsConference))

                .ForMember(dest => dest.TriesFor, expression => expression.MapFrom(
                    src => src.TriesFor))

                .ForMember(dest => dest.TriesAgainst, expression => expression.MapFrom(
                    src => src.TriesAgainst))

                .ForMember(dest => dest.TriesBonusPoints, src => src.UseValue(0))

                .ForMember(dest => dest.LossBonusPoints, src => src.UseValue(0))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
