namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;

    public class RugbyTeam
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyTeamId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderTeamId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Abbreviation { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LogoUrl { get; set; }

        public virtual DataProvider DataProvider { get; set; }
    }
}