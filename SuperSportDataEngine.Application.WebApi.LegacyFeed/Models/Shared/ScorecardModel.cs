using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class ScorecardModel
    {
        public List<Innings> Inningses { get; set; }
    }

    [Serializable]
    public class Innings: TeamModel
    {
        public int Number { get; set; }
        public int Runs { get; set; }
        public string Overs { get; set; }
        public int Wickets { get; set; }
        public string MaxOvers { get; set; }
        public bool Declared { get; set; }
        public string Minutes { get; set; }
        public int Extras { get; set; }
        public int Wides { get; set; }
        public int NoBalls { get; set; }
        public int Byes { get; set; }
        public int LegByes { get; set; }
        public List<Batter> Batsman { get; set; }
        public List<Bowler> Bowlers { get; set; }
        public List<Wicket> FallOfWickets { get; set; }
        public List<Partnership> PartnerShips { get; set; }
        public List<MatchEvent> Commentary { get; set; }
    }

    [Serializable]
    public class Batter : PersonModel
    {
        public int Order { get; set; }
        public int Runs { get; set; }
        public int Balls { get; set; }
        public int Fours { get; set; }
        public int Sixes { get; set; }
        public int Minutes { get; set; }
        public string HowOut { get; set; }
        public string HowOutText { get; set; }
        public string BowlerName { get; set; }
        public string FielderName { get; set; }
        public string StrikeRate { get; set; }
    }

    [Serializable]
    public class Bowler : PersonModel
    {
        public int Order { get; set; }
        public string Overs { get; set; }
        public int Maidens { get; set; }
        public int Runs { get; set; }
        public int Wickets { get; set; }
        public int Wides { get; set; }
        public int NoBalls { get; set; }
        public string Economy { get; set; }
    }

    [Serializable]
    public class Wicket: PersonModel
    {
        public int Order { get; set; }
        public string Overs { get; set; }
        public int Runs { get; set; }
    }

    [Serializable]
    public class Partnership
    {
        public Player Batter1 { get; set; }
        public Player Batter2 { get; set; }
        public int Order { get; set; }
        public int Runs { get; set; }
        public int Batter1Runs { get; set; }
        public int Batter2Runs { get; set; }
        public string Overs { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
