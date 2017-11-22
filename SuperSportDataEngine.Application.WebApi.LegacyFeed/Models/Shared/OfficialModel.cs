namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class OfficialModel: PersonModel
    {
        public string Role {get;set;}
    }
}