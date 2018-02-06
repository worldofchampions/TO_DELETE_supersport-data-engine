namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels
{
    public class MotorDriverRequestEntity
    {
        public MotorDriverRequestEntity(string providerSlug, int providerSeasonId)
        {
            ProviderSlug = providerSlug;
            ProviderSeasonId = providerSeasonId;
        }

        public string ProviderSlug { get; set; }
        public int ProviderSeasonId { get; set; }
    }
}