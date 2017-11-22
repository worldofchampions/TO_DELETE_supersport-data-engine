namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    public enum RetrievalType
    {
        TournamentID,
        LeagueID,
        Category
    }

    public enum PartnerSite
    {
        KaizerChiefs = 22,
        Pirates = 23,
        Sundowns = 24,
        SuperSportUnited = 25,
        JomoCosmos = 26,
        Kenya = 61,
        FAZ = 77,
        BloemCeltic = 79,
        MorokaSwallows = 65
    }

    public enum SportType
    {
        Rugby = 2,
        Cricket = 3,
        Football = 4,
        Golf = 5,
        MotorSport = 6,
        Tennis = 7,
        Basketball = 224
    }

    public enum GolfRankingsType
    {
        EuroMoneyList,
        FedexCup,
        MensRankings,
        USMoneyList,
        USSeniorMoneyList,
        WomensRankings
    }

    public enum CricketRankingsType
    {
        TestRankings,
        TestBatting,
        TestBowling,
        TestAllRounder,
        ODIRankings,
        ODIBatting,
        ODIBowling,
        ODIAllRounder,
        T20Rankings,
        T20Batting,
        T20Bowling,
        T20AllRounder,
    }
}