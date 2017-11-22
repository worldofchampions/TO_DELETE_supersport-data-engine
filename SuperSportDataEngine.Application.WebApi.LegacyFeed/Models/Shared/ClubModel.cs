namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class ClubModel
    {
        #region -- Properties --

        #region -- Id's --

        public int Id { get; set; }
        public int ParentId { get; set; }

        #endregion -- Id's --

        #region -- Names --

        public string Name { get; set; }
        public string ShortName { get; set; }
        public string NickName { get; set; }
        public string MobileName { get; set; }

        #endregion -- Names --

        #region -- Logos --

        public string LogoImageUrlLocal { get; set; }
        public string LogoImageUrlRemote { get; set; }
        public string SmallLogoName { get; set; }
        public string MediumLogoName { get; set; }
        public string LargeLogoName { get; set; }

        #endregion -- Logos --

        #endregion -- Properties --
    }
}