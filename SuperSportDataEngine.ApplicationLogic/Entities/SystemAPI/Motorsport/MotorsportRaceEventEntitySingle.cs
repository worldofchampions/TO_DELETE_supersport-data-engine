using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportRaceEventEntitySingle
    {
        /// <summary>
        /// The motorsport race event.
        /// </summary>
        public MotorsportRaceEventEntity RaceEvent { get; set; }
        /// <summary>
        /// List of available race event statuses.
        /// </summary>
        public List<EnumValue> MotorsportRaceEventStatuses
        {
            get
            {
                List<EnumValue> values = new List<EnumValue>();
                foreach (var itemType in Enum.GetValues(typeof(MotorsportRaceEventStatus)))
                {
                    //For each value of this enumeration, add a new EnumValue instance
                    values.Add(new EnumValue()
                    {
                        Name = Enum.GetName(typeof(MotorsportRaceEventStatus), itemType),
                        Value = (int)itemType
                    });
                }
                return values;
            }
        }
    }

    public class EnumValue
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
