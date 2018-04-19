namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    // This class is used by Reflection.
    public class LegacyMotorsportResultMapperProfile : Profile
    {
        public LegacyMotorsportResultMapperProfile()
        {
            CreateMap<MotorsportRaceEventResult, ResultMotorsport>()
                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.Position))

                .ForMember(dest => dest.GridPosition, expression => expression.MapFrom(
                    src => src.GridPosition))

                .ForMember(dest => dest.PositionText, expression => expression.UseValue((string)null))

                .ForMember(dest => dest.Laps, expression => expression.MapFrom(
                    src => src.LapsCompleted))

                .ForMember(dest => dest.GapToCarInFront, expression => expression.UseValue(string.Empty))

                .ForMember(dest => dest.GapToLeader, expression => expression.UseValue(string.Empty))

                .ForMember(dest => dest.Incomplete, expression => expression.MapFrom(
                    src => !src.CompletedRace))

                .ForMember(dest => dest.IncompleteReason, expression => expression.MapFrom(
                    src => string.IsNullOrEmpty(src.OutReason) ? "" : src.OutReason))

                .ForMember(dest => dest.Time, expression => expression.MapFrom(
                    src => !src.CompletedRace ? "" : GetTime(src)))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.Points))

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

                .ForMember(dest => dest.TeamCompetition, expression => expression.UseValue((string)null))

                .ForMember(dest => dest.TeamCompetitionId, expression => expression.UseValue(0))

                .ForAllOtherMembers(m => m.Ignore());
        }

        private static string GetTime(MotorsportRaceEventResult result)
        {
            if (result.Position == 1)
            {
                var hours = result.FinishingTimeHours.ToString().PadLeft(1, '0') + ":";
                var minutes = result.FinishingTimeMinutes.ToString().PadLeft(2, '0') + ":";
                var seconds = result.FinishingTimeSeconds.ToString().PadLeft(2, '0') + ".";
                var milliseconds = result.FinishingTimeMilliseconds.ToString().PadLeft(3, '0');

                return hours + minutes + seconds + milliseconds;
            }
            if (result.LapsBehind != 0)
            {
                return result.LapsBehind > 1 ? $"+{result.LapsBehind} Laps" : $"+{result.LapsBehind} Lap";
            }
            {
                var seconds = result.GapToLeaderTimeSeconds.ToString().PadLeft(2, '0');
                var milliseconds = result.GapToLeaderTimeMilliseconds.ToString().PadLeft(3, '0');

                return $"+{seconds}.{milliseconds}";
            }
        }
    }
}