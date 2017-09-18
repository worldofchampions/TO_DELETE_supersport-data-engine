namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyVenue
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderVenueId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}