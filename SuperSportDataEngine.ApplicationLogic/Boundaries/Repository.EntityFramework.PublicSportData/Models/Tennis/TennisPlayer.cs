using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisPlayer : BaseModel
    {
        /// <summary>
        /// A unique identifier for this TennisPlayer.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A unique identifier for the TennisPlayer for legacy purposes.
        /// </summary>
        public int LegacyPlayerId { get; set; }
        /// <summary>
        /// A provider driven value for the TennisPlayer.
        /// </summary>
        public int ProviderPlayerId { get; set; }
        /// <summary>
        /// A value indicating the first name of this TennisPlayer.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// A CMS driven value indicating the first name of this TennisPlayer.
        /// </summary>
        public string FirstNameCmsOverride { get; set; }
        /// <summary>
        /// A value indicating the last name of this TennisPlayer.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// A CMS driven value indicating the last name of this TennisPlayer.
        /// </summary>
        public string LastNameCmsOverride { get; set; }
        /// <summary>
        /// A value indicating whether the TennisPlayer is Right or Left handed.
        /// </summary>
        public TennisHandedness Handedness { get; set; }
        /// <summary>
        /// The gender of the TennisPlayer.
        /// </summary>
        public TennisGender Gender { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property indicating which country this TennisPlayer is from.
        /// </summary>
        public virtual TennisCountry TennisCountry { get; set; }
    }
}
