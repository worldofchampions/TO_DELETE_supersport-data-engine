using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums
{
    // Note: If/when extending this, do not change any existing enum int values, as these are being persisted at database level.
    public enum TennisRankingType
    {
        Unknown = 0,
        Rank = 1,
        Race = 2
    }
}
