using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisSide : BaseModel
    {
        /// <summary>
        /// A unique identifier for this TennisSide.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The side number. First or Second side.
        /// </summary>
        public int SideNumber { get; set; }
        /// <summary>
        /// Has this side won the match.
        /// </summary>
        public bool HasSideWon { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A value indicating the status of the TennisSide.
        /// Examples
        /// - Retired
        /// - Disqualified
        /// </summary>
        public string SideStatus { get; set; }
        /// <summary>
        /// The first TennisPlayer on this TennisSide.
        /// </summary>
        public virtual TennisPlayer TennisPlayerA { get; set; }
        /// <summary>
        /// The second TennisPlayer on this TennisSide.
        /// </summary>
        public virtual TennisPlayer TennisPlayerB { get; set; }
    }
}
