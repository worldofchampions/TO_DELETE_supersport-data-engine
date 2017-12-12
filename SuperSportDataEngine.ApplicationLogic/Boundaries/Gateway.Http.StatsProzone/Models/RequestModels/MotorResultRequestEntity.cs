namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels
{
    public class MotorResultRequestEntity
    {
        public MotorResultRequestEntity(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            ProviderSlug = providerSlug;
            ProviderSeasonId = providerSeasonId;
            ProviderRaceId = providerRaceId;
        }

        public string ProviderSlug { get;  set; }
        public int ProviderSeasonId { get;  set; }
        public int ProviderRaceId { get;  set; }
    }
}