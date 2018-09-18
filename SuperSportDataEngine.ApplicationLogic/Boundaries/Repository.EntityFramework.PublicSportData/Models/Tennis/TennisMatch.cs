using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisMatch : BaseModel
    {
        /// <summary>
        /// A unique Id for this TennisMatch.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A unique Id for Legacy purposes.
        /// </summary>
        public int LegacyMatchId { get; set; }
        /// <summary>
        /// A unique provider driven value for this TennisMatch.
        /// </summary>
        public int ProviderMatchId { get; set; }
        /// <summary>
        /// The round number that this match is part of.
        /// </summary>
        public int RoundNumber { get; set; }
        /// <summary>
        /// A provider driven value for the round type.
        /// </summary>
        public string RoundType { get; set; }
        /// <summary>
        /// The number of sets in this match.
        /// </summary>
        public int NumberOfSets { get; set; }
        /// <summary>
        /// The draw number for this match.
        /// </summary>
        public int DrawNumber { get; set; }
        /// <summary>
        /// The provider match status.
        /// </summary>
        public TennisMatchStatus TennisMatchStatus { get; set; }
        /// <summary>
        /// The Start date time of the TennisMatch.
        /// </summary>
        public DateTimeOffset StartDateTimeUtc { get; set; }
        /// <summary>
        /// The makeup date time of the TennisMatch.
        /// </summary>
        public DateTimeOffset? MakeupDateTimeUtc { get; set; }
        /// <summary>
        /// The court name where this TennisMatch is being played.
        /// </summary>
        public string CourtName { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// The associated TennisEvent TennisLeague Mapping.
        /// </summary>
        public virtual TennisEventTennisLeagues AssociatedTennisEventTennisLeague { get; set; }
        /// <summary>
        /// The first side for this TennisMatch.
        /// </summary>
        public virtual TennisSide TennisSideA { get; set; }
        /// <summary>
        /// The second side for this TennisMatch.
        /// </summary>
        public virtual TennisSide TennisSideB { get; set; }
        /// <summary>
        /// A list of TennisSets for this TennisSide.
        /// </summary>
        public virtual ICollection<TennisSet> TennisSets { get; set; }
    }
}
