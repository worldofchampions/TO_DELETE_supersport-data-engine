using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisLeague : BaseModel
    {
        /// <summary>
        /// A unique identifier for this Tennis league.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A unique identifier for legacy purposes.
        /// </summary>
        public int LegacyLeagueId { get; set; }
        /// <summary>
        /// A provider identifier for this league.
        /// </summary>
        public int ProviderLeagueId { get; set; }
        /// <summary>
        /// A CMS driven value for the Slug name.
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// a provider driven value for the Slug of this league.
        /// </summary>
        public string ProviderSlug { get; set; }
        /// <summary>
        /// A provider driven value for the name of this league.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The CMS driven value for the name of this league.
        /// </summary>
        public string NameCmsOverride { get; set; }
        /// <summary>
        /// A provider driven value for the abbreviation of this league.
        /// </summary>
        public string Abbreviation { get; set; }
        /// <summary>
        /// A CMS driven value to indicate whether this league should be polled from the Provider.
        /// </summary>
        public bool IsDisabledInbound { get; set; }
        /// <summary>
        /// A ingest driven value for the data provider.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// The Gender of this TennisLeague
        /// </summary>
        public TennisGender Gender { get; set; }

        /// <summary>
        /// A league can contain many tournaments.
        /// </summary>
        public virtual ICollection<TennisTournament> TennisTournaments { get; set; }
    }
}
