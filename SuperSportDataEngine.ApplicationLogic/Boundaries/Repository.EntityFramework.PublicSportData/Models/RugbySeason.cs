namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
    using System;

    public class RugbySeason : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderSeasonId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value to set whether the season is current. </summary>
        public bool IsCurrent { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset EndDateTime { get; set; }

        /// <summary> A CMS driven value to set the corresponding RugbyLogType. </summary>
        public RugbyLogType RugbyLogType { get; set; }

        public DataProvider DataProvider { get; set; }

        /// <summary> A provider driven value for the current round numebr for the season. </summary>
        public int CurrentRoundNumber { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }
    }
}