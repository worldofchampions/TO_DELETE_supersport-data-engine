using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class Scorer: PersonModel
    {
        public string Type { get; set; }
        public int EventId { get; set; }
        public int Time { get; set; }
    }
}
