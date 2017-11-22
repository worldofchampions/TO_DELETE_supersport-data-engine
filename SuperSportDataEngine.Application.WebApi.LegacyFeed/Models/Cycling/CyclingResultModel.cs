namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using System;

    [Serializable]
    public class CyclingResultModel : PersonModel
    {
        public string Gap { get; set; }
        public int Position { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string Time { get; set; }
    }
}