namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class ClubDetailsModel : ClubModel
    {
        #region -- Properties --

        #region -- Kits --

        public string KitImageUrlLocal { get; set; }
        public string KitImageUrlRemote { get; set; }
        public string HomeKitName { get; set; }
        public string AwayKitName { get; set; }

        #endregion

        #region -- About --

        public string About { get; set; }
        public string Founded { get; set; }
        public string Honours { get; set; }
        public string HomeGround { get; set; }
        public string Sponsor { get; set; }

        #endregion

        #region -- Personel --

        public string Chairman { get; set; }
        public string TeamManager { get; set; }
        public string Coach { get; set; }
        public string Assistant { get; set; }
        public string Captain { get; set; }

        #endregion

        #region -- Contact Details --

        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Fax { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string GoogleMap { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }

        #endregion

        #endregion
    }
}