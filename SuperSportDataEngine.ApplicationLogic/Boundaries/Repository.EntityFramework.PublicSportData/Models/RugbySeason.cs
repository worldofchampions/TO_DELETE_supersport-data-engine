namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
    using System;

    public class RugbySeason
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderSeasonId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A provider driven value. </summary>
        public bool IsCurrent { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset EndDateTime { get; set; }

        /// <summary> A CMS driven value to set the LogType. </summary>
        public RugbyLogType RugbyLogType { get; set; }

        public DataProvider DataProvider { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }
    }
}