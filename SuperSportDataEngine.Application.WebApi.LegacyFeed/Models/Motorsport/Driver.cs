namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class Driver : Team
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
