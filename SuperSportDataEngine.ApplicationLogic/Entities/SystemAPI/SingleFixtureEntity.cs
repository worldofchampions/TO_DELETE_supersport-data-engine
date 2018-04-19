using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class SingleFixtureEntity
    {
        /// <summary>
        /// The rugby fixture.
        /// </summary>
        public RugbyFixtureEntity Fixture { get; set; }
        /// <summary>
        /// The records this page represents.
        /// </summary>
        public List<EnumValue> RugbyFixtureStatuses
        {
            get
            {
                List<EnumValue> values = new List<EnumValue>();
                foreach (var itemType in Enum.GetValues(typeof(RugbyFixtureStatus)))
                {
                    //For each value of this enumeration, add a new EnumValue instance
                    values.Add(new EnumValue()
                    {
                        Name = Enum.GetName(typeof(RugbyFixtureStatus), itemType),
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
