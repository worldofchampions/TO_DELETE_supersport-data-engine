namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;

    public class RugbyVenue
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderVenueId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        public virtual DataProvider DataProvider { get; set; }
    }
}