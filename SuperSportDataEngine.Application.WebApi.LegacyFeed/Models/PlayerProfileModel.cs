using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class PlayerProfileModel: PersonModel
    {
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Country { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Position { get; set; }
        public string Biography { get; set; }
        public string SmallImage { get; set; }
        public string LargeImage { get; set; }
        public string ProvincialImageSmall { get; set; }
        public string ProvincialImageLarge { get; set; }
        public string IpadImageSmall { get; set; }
        public string IpadImageLarge { get; set; }
        public string CurrentTeam { get; set; }

        public List<TeamModel> PreviousTeams { get; set; }
    }
}
