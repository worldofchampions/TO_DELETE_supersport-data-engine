using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisSurfaceType : BaseModel
    {
        /// <summary>
        /// A unique identifier for this TennisSurfaceType
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A provider driven value for SurfaceType for lookup purposes.
        /// </summary>
        public int ProviderSurfaceId { get; set; }
        /// <summary>
        /// A value indicating the name of this surface type.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A CMS driven value for the override value of the name.
        /// </summary>
        public string NameCmsOverride { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
    }
}
