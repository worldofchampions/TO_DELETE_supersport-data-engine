namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public class MotorsportSeason: BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }
        
        /// <summary> The provider's record identifier. </summary>
        public int ProviderSeasonId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset EndDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public bool IsActive { get; set; }

        /// <summary> A CMS driven value to set whether the season is current. </summary>
        public bool IsCurrent { get; set; }

        public DataProvider DataProvider { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }
    }
}