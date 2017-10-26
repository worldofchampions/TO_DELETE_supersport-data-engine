using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy
{
    [NotMapped]
    public class LegacyRugbyMatchEventEntity: RugbyMatchEvent
    {
        public string Comments { get; set; }
    }
}
