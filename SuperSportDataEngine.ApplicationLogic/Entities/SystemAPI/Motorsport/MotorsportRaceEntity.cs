using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportRaceEntity
    {
        public Guid Id { get; set; }
        public int LegacyRaceId { get; set; }
        public int ProviderRaceId { get; set; }
        public string RaceName { get; set; }
        public string RaceNameCmsOverride { get; set; }
        public string RaceNameAbbreviation { get; set; }
        public string RaceNameAbbreviationCmsOverride { get; set; }
        public bool IsDisabledInbound { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}
