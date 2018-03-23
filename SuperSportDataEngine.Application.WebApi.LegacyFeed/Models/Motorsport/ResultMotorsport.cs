namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class ResultMotorsport : Driver
    {
        public int Position { get; set; }
        public string PositionText { get; set; }
        public string Time { get; set; }
        public string Points { get; set; }
        public string GapToCarInFront { get; set; }
        public string GapToLeader { get; set; }
        public string FastestTime { get; set; }
        public string FastestLap { get; set; }
        public bool Incomplete { get; set; }
        public string IncompleteReason { get; set; }
        public int GridPosition { get; set; }
        public int Pits { get; set; }
        public int Laps { get; set; }
        public string Comment { get; set; }
    }
}
