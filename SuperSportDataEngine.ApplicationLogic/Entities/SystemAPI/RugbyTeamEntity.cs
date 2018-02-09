using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyTeamEntity
    {
        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyTeamId { get; set; }

        /// <summary> The provider's record identifier. </summary>
        public int ProviderTeamId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string NameCmsOverride { get; set; } = null;

        /// <summary> A provider driven value. </summary>
        public string Abbreviation { get; set; }

        /// <summary> A provider driven value. </summary>
        public string LogoUrl { get; set; }
    }
}
