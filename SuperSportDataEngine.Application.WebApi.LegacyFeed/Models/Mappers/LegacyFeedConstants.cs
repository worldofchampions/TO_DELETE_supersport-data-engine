using System.Collections.Generic;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public static class LegacyFeedConstants
    {
        private const string _emptyString = "";

        public const int GroupHiearachyLevelZero = 0;

        public const int GroupHiearachyLevelOne = 1;

        public const int GroupHiearachyLevelTwo = 2;

        public static int UnknownFixtureStatusId = 0;

        public static string UnknownFixtureStatusDescription = _emptyString;

        public static int PreMatchStatusId = 0;

        public const string PreMatchDescription = _emptyString;

        public const int FirstHalfStatusId = 4;

        public const string FirstHalfDescription = "First Half";

        public static int HalfTimeStatusId = FirstHalfStatusId;

        public const string HalfTimeDescription = "Half Time";

        public const int SecondHalfStatusId = 7;

        public const string SecondHalfDescription = "Second Half";

        public const int ResultsStatusId = 14;

        public const string ResultsDescription = "Result";

        public static int FullTimeStatusId = ResultsStatusId;

        public static int ExtraTimeStatusId = UnknownFixtureStatusId;

        public const string FullTimeDescription = ResultsDescription;

        private const string ExtraTimeDescription = "Extra Time";

        public const int CommentaryEventId = 0;

        public const string DefaultAttendanceValue = "0";

        public const string EmptyPlayerName = _emptyString;

        public const string EmptyEventName = _emptyString;

        public const string EmptyEventComment = _emptyString;

        public const string EmptyTeamName = _emptyString;

        public const int DefaultScoreForStartedGame = 0;

        public const int DefaultSortingValue = 0;

        public const string GeneralResponseMessage = "No items available";

        public static List<string> EmptyChannelsList
        {
            get
            {
                return Enumerable.Empty<string>().ToList();
            }
        }

        public static List<MatchVideoModel> EmptyVideosList
        {
            get
            {
                return Enumerable.Empty<MatchVideoModel>().ToList();
            }
        }

        public static List<MatchLiveVideoModel> EmptyLiveVideosList
        {
            get
            {
                return Enumerable.Empty<MatchLiveVideoModel>().ToList();
            }
        }

        public static List<OfficialModel> EmptyMatchOfficialsList
        {
            get
            {
                return Enumerable.Empty<OfficialModel>().ToList();
            }
        }

        public static List<CardsModel> EmptyTeamCardsList
        {
            get
            {
                return Enumerable.Empty<CardsModel>().ToList();
            }
        }

        public static List<SubstituteModel> EmptyTeamSubstitutes
        {
            get
            {
                return Enumerable.Empty<SubstituteModel>().ToList();
            }
        }

        public static string GetFixtureStatusDescription(RugbyFixtureStatus rugbyFixtureStatus)
        {
            switch (rugbyFixtureStatus)
            {
                case RugbyFixtureStatus.PreMatch:
                    return PreMatchDescription;

                case RugbyFixtureStatus.FirstHalf:
                    return FirstHalfDescription;

                case RugbyFixtureStatus.HalfTime:
                    return HalfTimeDescription;

                case RugbyFixtureStatus.SecondHalf:
                    return SecondHalfDescription;

                case RugbyFixtureStatus.FullTime:

                case RugbyFixtureStatus.Result:
                    return ResultsDescription;

                case RugbyFixtureStatus.ExtraTime:
                    return ExtraTimeDescription;

                default:
                    return UnknownFixtureStatusDescription;
            }
        }

        public static int GetFixtureStatusId(RugbyFixtureStatus rugbyFixtureStatus)
        {
            switch (rugbyFixtureStatus)
            {
                case RugbyFixtureStatus.PreMatch:
                    return PreMatchStatusId;

                case RugbyFixtureStatus.FirstHalf:
                    return FirstHalfStatusId;

                case RugbyFixtureStatus.HalfTime:
                    return HalfTimeStatusId;

                case RugbyFixtureStatus.SecondHalf:
                    return SecondHalfStatusId;

                case RugbyFixtureStatus.FullTime:

                case RugbyFixtureStatus.Result:
                    return ResultsStatusId;

                case RugbyFixtureStatus.ExtraTime:
                    return ExtraTimeStatusId;

                default:
                    return UnknownFixtureStatusId;
            }
        }
    }
}