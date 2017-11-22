namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public abstract class PersonModel
    {
        private string _name;
        private string _surname;
        public int PersonId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string NickName { get; set; }

        public string CombinedName { get; set; }
        public string DisplayName { get; set; }
    }
}