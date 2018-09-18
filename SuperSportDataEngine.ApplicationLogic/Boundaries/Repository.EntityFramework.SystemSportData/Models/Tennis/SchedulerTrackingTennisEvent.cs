using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis
{
    public class SchedulerTrackingTennisEvent
    {
        /// <summary>
        /// A unique identifier identifying this tracking entry.
        /// </summary>
        public Guid TennisEventId { get; set; }
        /// <summary>
        /// The start date of this TennisEvent.
        /// </summary>
        public DateTimeOffset StartDateTime { get; set; }
        /// <summary>
        /// The end date of this TennisEvent.
        /// </summary>
        public DateTimeOffset EndDateTime { get; set; }
    }
}
