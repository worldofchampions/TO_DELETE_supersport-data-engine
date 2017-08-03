using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class MarginModel
    {
        public int TeamId
        {
            get;
            set;
        }

        public string TeamName
        {
            get;
            set;
        }

        public int Points
        {
            get;
            set;
        }
    }
}
