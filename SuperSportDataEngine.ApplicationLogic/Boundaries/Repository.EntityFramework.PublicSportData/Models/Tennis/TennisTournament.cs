using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisTournament : BaseModel
    {
        /// <summary> 
        /// The primary internal record identifier.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary> 
        /// A unique int record identifier for legacy purposes. 
        /// </summary>
        public int LegacyTournamentId { get; set; }
        /// <summary> 
        /// The provider's record identifier.
        /// </summary>
        public int ProviderTournamentId { get; set; }
        /// <summary> 
        /// A CMS driven value for the Slug of the TennisTournament.
        /// </summary>
        public string Slug { get; set; }
        /// <summary> 
        /// The provider driven value for the name of the TennisTournament.
        /// </summary>
        public string ProviderTournamentName { get; set; }
        /// <summary> 
        /// The provider driven value for the display name.
        /// </summary>
        public string ProviderDisplayName { get; set; }
        /// <summary> 
        /// A CMS driven value for the override for the display name.
        /// </summary>
        public string NameCmsOverride { get; set; }
        /// <summary> 
        /// The provider driven value for the abbreviation for the tournament.
        /// </summary>
        public string Abbreviation { get; set; }
        /// <summary>
        /// A provider driven value for the type of TennisEvent.
        /// </summary>
        public TennisTournamentType TennisTournamentType { get; set; }
        /// <summary> 
        /// A CMS driven value to indicate if a tournament should be queried from the provider.
        /// </summary>
        public bool IsDisabledInbound { get; set; }
        /// <summary> 
        /// A CMS driven value to indicate if a tournament should be queried from the consumers.
        /// </summary>
        public bool IsDisabledOutbound { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary> 
        /// A Tennis Tournament can be a part of multiple Tennis Leagues.
        /// </summary>
        public virtual ICollection<TennisLeague> TennisLeagues { get; set; }
    }
}
