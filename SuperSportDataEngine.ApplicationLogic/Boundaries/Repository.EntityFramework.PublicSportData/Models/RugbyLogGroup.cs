namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyLogGroup
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int? ProviderLogGroupId { get; set; }

        public DataProvider DataProvider { get; set; }

        public string GroupName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public int GroupHierarchyLevel { get; set; }

        /// <summary> A CMS driven value. </summary>
        public RugbyLogGroup ParentRugbyLogGroup { get; set; }
    }
}