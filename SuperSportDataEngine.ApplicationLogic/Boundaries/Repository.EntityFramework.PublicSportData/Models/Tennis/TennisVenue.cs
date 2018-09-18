using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisVenue : BaseModel
    {
        /// <summary>
        /// Primary identifier for this venue.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// An internal identifier for this venue for legacy purposes.
        /// </summary>
        public int LegacyVenueId { get; set; }
        /// <summary>
        /// A provider driven value to indicate the identifier for this venue.
        /// </summary>
        public int ProviderVenueId { get; set; }
        /// <summary>
        /// A provider driven value for the name of this venue.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A CMS driven value to indicate the overridden value for the name of this venue.
        /// </summary>
        public string NameCmsOverride { get; set; }
        /// <summary>
        /// A provider driven value to indicate the city of this venue.
        /// </summary>
        public string City { get; set; }
        
        /// <summary>
        /// An ingest driven value to indicate the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property indicating which country this TennisVenue is located.
        /// </summary>
        public virtual TennisCountry TennisCountry { get; set; }
    }
}
