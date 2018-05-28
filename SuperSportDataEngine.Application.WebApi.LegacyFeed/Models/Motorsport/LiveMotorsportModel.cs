namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class LiveMotorsportModel
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public int SessionId { get; set; }
        public string RaceName { get; set; }
        public string RaceVenue { get; set; }
        public string SessionName { get; set; }
        public int SessionNumber { get; set; }
        public string SessionType { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime SessionTimeStamp { get; set; }
        public int SessionLaps { get; set; }
        public int SessionTotalLaps { get; set; }
        public int SessionMinutes { get; set; }
        public string SessionStatus { get; set; }
        public string SessionComments { get; set; }
        public List<ResultMotorsport> Leaderboard { get; set; }
        public List<CommentModel> Commentary { get; set; }
    }
}
