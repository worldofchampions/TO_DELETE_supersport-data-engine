using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisSet : BaseModel
    {
        /// <summary>
        /// A unique identifier for this TennisSet.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The SetNumber in the TennisMatch.
        /// </summary>
        public int SetNumber { get; set; }
        /// <summary>
        /// Indicates whether the TennisSide has Won this TennisSet.
        /// </summary>
        public bool SideAHasWon { get; set; }
        /// <summary>
        /// Indicates whether the TennisSide has Won this TennisSet.
        /// </summary>
        public bool SideBHasWon { get; set; }
        /// <summary>
        /// The number of games won in this TennisSet.
        /// </summary>
        public int SideAGamesWon { get; set; }
        /// <summary>
        /// The number of games won in this TennisSet.
        /// </summary>
        public int SideBGamesWon { get; set; }
        /// <summary>
        /// A value to indicate whether this TennisSet is a tie breaker.
        /// </summary>
        public bool SetIsTieBreaker { get; set; }

        /// <summary>
        /// The Tie-Break for Side A.
        /// </summary>
        public int? SideATieBreakerPoints { get; set; }
        /// <summary>
        /// The Tie-Break for Side B.
        /// </summary>
        public int? SideBTieBreakerPoints { get; set; }
        /// <summary>
        /// The Data Provider for where this data is coming from.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property for the TennisSide.
        /// </summary>
        public virtual TennisSide TennisSideA { get; set; }
        /// <summary>
        /// A navigation property for the TennisSide.
        /// </summary>
        public virtual TennisSide TennisSideB { get; set; }
    }
}
