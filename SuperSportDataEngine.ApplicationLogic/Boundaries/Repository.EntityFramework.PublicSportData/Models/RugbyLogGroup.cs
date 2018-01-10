namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyLogGroup : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's group id value. For internal look-up purposes. </summary>
        public int? ProviderLogGroupId { get; set; }

        /// <summary> The provider's group name value. For internal look-up purposes. </summary>
        public string ProviderGroupName { get; set; }

        public DataProvider DataProvider { get; set; }

        /// <summary> A CMS driven value. Public facing value. </summary>
        public string GroupName { get; set; }

        /// <summary> A CMS driven value. Public facing value. </summary>
        public string GroupShortName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public int GroupHierarchyLevel { get; set; }

        /// <summary> A CMS driven value that uniquely identify a record to facilitate tracking and hierarchy assignment purposes. </summary>
        public string Slug { get; set; }

        /// <summary> A CMS driven value. </summary>
        public bool IsConference { get; set; }

        /// <summary> A CMS driven value. </summary>
        public RugbyLogGroup ParentRugbyLogGroup { get; set; }

        public virtual RugbySeason RugbySeason { get; set; }
    }
}