namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;

    [Serializable]
    public class DriverModel : TeamModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initials { get; set; }
        public string Abbreviation { get; set; }
        public string FullName { get; set; }
        public string CarNumber { get; set; }
        public string Country { get; set; }
    }
}