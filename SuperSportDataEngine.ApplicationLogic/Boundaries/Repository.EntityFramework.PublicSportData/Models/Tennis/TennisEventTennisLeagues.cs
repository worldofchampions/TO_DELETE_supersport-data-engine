using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisEventTennisLeagues : BaseModel
    {
        /// <summary>
        /// A clustered record identifier for this mapping.
        /// </summary>
        public Guid TennisLeagueId { get; set; }
        /// <summary>
        /// A clustered record identifier for this mapping.
        /// </summary>
        public Guid TennisEventId { get; set; }
        /// <summary>
        /// The provider driven value for the prize of this TennisEvent.
        /// </summary>
        public string Prize { get; set; }
        /// <summary>
        /// The CMS driven value for the prize of this TennisEvent.
        /// </summary>
        public string PrizeCmsOverride { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property for the TennisEvent.
        /// </summary>
        public virtual TennisEvent TennisEvent { get; set; }
        /// <summary>
        /// A navigation property for the TennisLeague.
        /// </summary>
        public virtual TennisLeague TennisLeague { get; set; }
    }
}
