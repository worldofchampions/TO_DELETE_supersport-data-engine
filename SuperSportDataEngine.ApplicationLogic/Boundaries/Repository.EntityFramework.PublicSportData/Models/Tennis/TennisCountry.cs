using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisCountry : BaseModel
    {
        /// <summary>
        /// A unique identifier for this TennisCountry.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A provider driven value for the TennisCountry.
        /// </summary>
        public int ProviderCountryId { get; set; }
        /// <summary>
        /// A provider driven value to indicate the Country of this venue.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// A provider driven value for the abbreviation of this venue.
        /// </summary>
        public string CountryAbbreviation { get; set; }
        /// <summary>
        /// A CMS driven value indicating the URL for the Country logo/flag.
        /// </summary>
        public string CountryLogoUrl { get; set; }
        /// <summary>
        /// An ingest driven value to indicate the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
    }
}
