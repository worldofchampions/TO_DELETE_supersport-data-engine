namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class VenueModel
    {
        #region -- Properties --

        #region -- Id's --

        public int Id { get; set; }
        public int ParentId { get; set; }

        #endregion -- Id's --

        #region -- Names --

        public string Name { get; set; }
        public string ShortName { get; set; }

        #endregion -- Names --

        #region -- Logos --

        public string ImageUrlLocal { get; set; }
        public string ImageUrlRemote { get; set; }
        public string SmallImage { get; set; }
        public string MediumImage { get; set; }
        public string LargeImage { get; set; }

        #endregion -- Logos --

        #endregion -- Properties --
    }
}