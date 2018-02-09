using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyPlayerEntity
    {
        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyPlayerId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderPlayerId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string FullName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string FirstName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LastName { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string DisplayNameCmsOverride { get; set; }

        public RugbyDataProviderEntity DataProvider { get; set; }
    }

    public enum RugbyDataProviderEntity
    {
        CmsDataCapture = 1,
        StatsProzone = 2
    }
}
