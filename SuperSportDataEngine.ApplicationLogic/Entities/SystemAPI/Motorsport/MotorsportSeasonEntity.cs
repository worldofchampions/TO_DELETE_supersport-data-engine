using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportSeasonEntity
    {
        public Guid Id { get; set; }
        public int ProviderSeasonId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }

        public DataProvider DataProvider { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }
    }
}
