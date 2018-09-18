using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisEventSeed : BaseModel
    {
        /// <summary>
        /// A clustered record identifier for this Event seed.
        /// </summary>
        public Guid TennisEventId { get; set; }
        /// <summary>
        /// A clustered record identifier for this Event seed.
        /// </summary>
        public Guid TennisPlayerId { get; set; }
        /// <summary>
        /// The seed value for this player in this TennisEvent.
        /// </summary>
        public int SeedValue { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property to the Tennis Event.
        /// </summary>
        public virtual TennisEvent TennisEvent { get; set; }
        /// <summary>
        /// A navigation property to the TennisPlayer.
        /// </summary>
        public virtual TennisPlayer TennisPlayer { get; set; }
    }
}
