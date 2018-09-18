using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis
{
    public class SchedulerTrackingTennisMatch
    {
        /// <summary>
        /// A unique identifer for this tracking event.
        /// </summary>
        public Guid TennisMatchId { get; set; }
        /// <summary>
        /// The start date time of this tennis match.
        /// </summary>
        public DateTimeOffset StartDateTime { get; set; }
        /// <summary>
        /// The end date time of this tennis match.
        /// </summary>
        public DateTimeOffset? EndDateTime { get; set; }
        /// <summary>
        /// The scheduler state for the match polling.
        /// </summary>
        public SchedulerStateForTennisMatchPolling SchedulerStateForTennisMatchPolling { get; set; }
    }
}
