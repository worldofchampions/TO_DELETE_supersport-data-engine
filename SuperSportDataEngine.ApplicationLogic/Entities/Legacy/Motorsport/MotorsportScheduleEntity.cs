using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport
{
    public class MotorsportScheduleEntity
    {
        public List<MotorsportRaceEvent> MotorsportRaceEvents { get; set; }
    }
}
