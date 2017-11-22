namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class ManagementModel : PersonModel
    {
        public string ManagementType { get; set; }
    }
}