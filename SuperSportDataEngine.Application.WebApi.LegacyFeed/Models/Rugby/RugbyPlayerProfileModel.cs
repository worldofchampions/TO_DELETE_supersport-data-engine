using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby
{
    [Serializable]
    public class RugbyPlayerProfileModel : PlayerProfileModel
    {
        public string CountryOfBirth { get; set; }
        public string PrimarySchool { get; set; }
        public string SecondarySchool { get; set; }
        public string TertiaryInstitution { get; set; }
        public string MaritalStatus { get; set; }
        public List<Favourite> Favourites { get; set; }
        public Career PlayerCareer { get; set; }
        public InternationalCareer PlayerInternationalCareer { get; set; }
        public List<InternationalRecord> PlayerInternationalRecord { get; set; }
    }

    [Serializable]
    public class Favourite
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [Serializable]
    public class Career
    {
        public int Tries { get; set; }
        public int Conversions { get; set; }
        public int Penalties { get; set; }
        public int DropGoals { get; set; }
        public int Points { get; set; }
    }

    [Serializable]
    public class InternationalCareer
    {
        public int Tries { get; set; }
        public int Conversions { get; set; }
        public int Penalties { get; set; }
        public int DropGoals { get; set; }
        public int Points { get; set; }
        public int Caps { get; set; }
    }

    [Serializable]
    public class InternationalRecord
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drew { get; set; }
        public int Lost { get; set; }
        public int Tries { get; set; }
        public int Conversions { get; set; }
        public int Penalties { get; set; }
        public int DropGoals { get; set; }
    }
}