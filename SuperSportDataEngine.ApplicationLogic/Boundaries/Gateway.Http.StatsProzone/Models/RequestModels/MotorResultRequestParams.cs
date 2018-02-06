namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels
{
    public class MotorResultRequestParams
    {
        public MotorResultRequestParams(string slug, int seasonId, int raceId)
        {
            Slug = slug;
            SeasonId = seasonId;
            RaceId = raceId;
        }

        public string Slug { get;  set; }
        public int SeasonId { get;  set; }
        public int RaceId { get;  set; }
    }
}