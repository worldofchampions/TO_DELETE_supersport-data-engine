using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisSeason : BaseModel
    {
        /// <summary>
        /// A globally unique identifier for this Tennis Season.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A provider driven value identifying the season.
        /// </summary>
        public int ProviderSeasonId { get; set; }
        /// <summary>
        /// A provider driven value for the name of this season.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A provider driven value to indicate whether the season is active.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// A CMS driven value for whether this season is current.
        /// </summary>
        public bool IsCurrent { get; set; }
        /// <summary>
        /// A provider driven value for the start date of the season.
        /// </summary>
        public DateTimeOffset StartDateUtc { get; set; }
        /// <summary>
        /// A provider driven value for the end date of the season.
        /// </summary>
        public DateTimeOffset EndDateUtc { get; set; }
        /// <summary>
        /// A value indicating the source of this data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property for the TennisLeague.
        /// </summary>
        public virtual TennisLeague TennisLeague { get; set; }
    }
}
