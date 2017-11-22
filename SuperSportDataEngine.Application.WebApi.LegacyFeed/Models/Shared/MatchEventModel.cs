using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class MatchEvent
    {
        public long Id { get; set; }
        public int Player1Id { get; set; }
        public string Player1FullName { get; set; }
        public string Player1Surname { get; set; }
        public string Player1DisplayName { get; set; }
        public int Player2Id { get; set; }
        public string Player2FullName { get; set; }
        public string Player2Surname { get; set; }
        public string Player2DisplayName { get; set; }
        public string EventName { get; set; }
        public int EventId { get; set; }
        public int TeamId { get; set; }
        public int Time { get; set; }
        public int ExtendTime { get; set; }
        public string DisplayTime { get; set; }
        public string Comments { get; set; }
        public int MatchId { get; set; }
    }
}
