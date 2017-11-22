namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class TeamModel
    {
        public int TeamId { get; set; }

        private string teamName { get; set; }
        public string TeamName { get; set; }
        public string TeamType { get; set; }
        public string TeamShortName { get; set; }
        public int TeamCompetitionId { get; set; }
        public string TeamCompetition { get; set; }
    }
}