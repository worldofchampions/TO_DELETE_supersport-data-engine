using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class PlayerProfileFullModel : PlayerProfileModel
    {

        #region -- Properties --

        public int TeamId { get; set; }
        public int Jersey { get; set; }	
        public int RoleId { get; set; }
        public string CountryOfBirth { get; set; }
        public string Foot { get; set; }		
        public string Role { get; set; }		
        public string PrimarySchool { get; set; }		
        public string SecondarySchool { get; set; }		
        public string TertiaryInstitution { get; set; }		
        public string MaritalStatus { get; set; }		
        public string FavouriteFilm { get; set; }
        public string FavouriteMusic { get; set; }
        public string FavouriteFood { get; set; }
        public string FavouritePlayer { get; set; }
        public string FavouriteOpponent { get; set; }		
        public string Car { get; set; }		
        public string RoleModel { get; set; }		
        public string Hobbies { get; set; }		
        public string Comments { get; set; }		
        public string Motto { get; set; }		
        public string Message { get; set; }		
        public string ClubsStr { get; set; }	
        public int Goals { get; set; }
        public int ClubGoals { get; set; }
        public int Caps { get; set; }
        public int ClubCaps { get; set; }

        #endregion

      

    }
}
