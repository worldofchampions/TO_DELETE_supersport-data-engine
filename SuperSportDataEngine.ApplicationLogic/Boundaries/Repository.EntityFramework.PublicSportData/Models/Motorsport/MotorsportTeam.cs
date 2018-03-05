namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public class MotorsportTeam: BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyTeamId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderTeamId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string NameCmsOverride { get; set; }

        public MotorsportLeague MotorsportLeague { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}