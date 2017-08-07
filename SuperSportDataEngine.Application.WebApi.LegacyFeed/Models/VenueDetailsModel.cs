using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class VenueDetailsModel : VenueModel
    {
        #region -- Properties --

        #region -- Addess --

        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Directions { get; set; }

        #endregion -- Addess --

        #region -- About --

        public int Capacity { get; set; }
        public string About { get; set; }

        #endregion -- About --

        #endregion -- Properties --
    }
}