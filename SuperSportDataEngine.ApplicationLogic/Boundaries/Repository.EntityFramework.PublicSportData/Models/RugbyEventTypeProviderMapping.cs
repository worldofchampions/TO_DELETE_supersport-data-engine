namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyEventTypeProviderMapping : BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public DataProvider DataProvider { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public int ProviderEventTypeId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string ProviderEventName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public Guid RugbyEventTypeId { get; set; }

        public virtual RugbyEventType RugbyEventType { get; set; }
    }
}