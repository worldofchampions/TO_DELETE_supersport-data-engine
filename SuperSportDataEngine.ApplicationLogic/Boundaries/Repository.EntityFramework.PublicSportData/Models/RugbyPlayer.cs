namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyPlayer
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyPlayerId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderPlayerId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string FullName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string FirstName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LastName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string DisplayNameCmsOverride { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}