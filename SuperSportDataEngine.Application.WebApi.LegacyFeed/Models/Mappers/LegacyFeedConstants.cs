using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public static class LegacyFeedConstants
    {
        private const string _emptyString = "";

        public const int GroupHiearachyLevelZero = 0;

        public const int GroupHiearachyLevelOne = 1;

        public const int GroupHiearachyLevelTwo = 2;

        public const int FirstHalfStatusId = 4;

        public const string FirstHalfStatusDescription = "First Half";

        public const int SecondHalfStatusId = 7;

        public const string SecondHalfStatusDescription = "Second Half";

        public const int CommentaryEventId = 0;

        public const string DefaultAttendanceValue = "0";

        public const string EmptyPlayerName = _emptyString;

        public const string EmptyEventName = _emptyString;

        public const string EmptyEventComment = _emptyString;

        public const string EmptyTeamName = _emptyString;

        public const int DefaultScoreForStartedGame = 0;

        public const int DefaultSortingValue = 0;

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
    }
}