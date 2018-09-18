using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisRanking : BaseModel
    {
        /// <summary>
        /// A unique identifier for the TennisPlayer.
        /// </summary>
        public Guid TennisPlayerId { get; set; }
        /// <summary>
        /// A unique identifier for the TennisLeauge.
        /// </summary>
        public Guid TennisLeagueId { get; set; }
        /// <summary>
        /// A unique identifier for the TennisSeason.
        /// </summary>
        public Guid TennisSeasonId { get; set; }
        /// <summary>
        /// The Tennis Ranking type.
        /// </summary>
        public TennisRankingType TennisRankingType { get; set; }
        /// <summary>
        /// The ranking of this TennisPlayer for the league.
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// A value indicating the number of points this TennisPlayer has for this TennisSeason and TennisLeague.
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// The movement of this TennisPlayer in the rankings.
        /// </summary>
        public int? Movement { get; set; }
        /// <summary>
        /// The date at which this TennisRanking was valid last at.
        /// </summary>
        public DateTimeOffset RankingValidLastAt { get; set; }
        /// <summary>
        /// A navigation property for this TennisPlayer.
        /// </summary>
        public virtual TennisPlayer TennisPlayer { get; set; }
        /// <summary>
        /// A navigation property for the TennisLeague.
        /// </summary>
        public virtual TennisLeague TennisLeague { get; set; }
        /// <summary>
        /// A navigation property for this TennisSeason.
        /// </summary>
        public virtual TennisSeason TennisSeason { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
    }
}
