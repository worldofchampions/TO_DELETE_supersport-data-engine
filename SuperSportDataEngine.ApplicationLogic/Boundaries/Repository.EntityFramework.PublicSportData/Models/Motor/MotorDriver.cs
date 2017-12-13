using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorDriver: BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyDriverId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderDriverId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string FirstName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LastName { get; set; }

        /// <summary> Provider does not serve this value. </summary>
        public string FullName { get; set; }

        /// <summary> A provider driven value. </summary>
        public double HeightInCentimeters { get; set; }

        /// <summary> A provider driven value. </summary>
        public double WeightInKilograms { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string DisplayNameCmsOverride { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CountryName { get; set; }

        /// <summary> A provider driven value. </summary>
        public int ProviderCarId { get; set; }

        /// <summary> A provider driven value. </summary>
        public int? CarNumber { get; set; }

        /// <summary> A provider driven value. </summary>
        public int? CarDisplayNumber { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CarName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CarDisplayName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string TeamName { get; set; }

        /// <summary> A provider driven value. </summary>
        public int ProviderTeamId { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}