namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class SubstituteModel
    {
        public int PersonOnId { get; set; }
        public string PersonOnName { get; set; }
        public int PersonOffId { get; set; }
        public string PersonOffName { get; set; }
        public int Time { get; set; }
        public string ReplacementType { get; set; }
        public int EventId { get; set; }
    }
}