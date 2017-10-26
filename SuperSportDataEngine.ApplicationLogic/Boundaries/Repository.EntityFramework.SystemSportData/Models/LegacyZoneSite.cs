namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class LegacyZoneSite : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sport { get; set; }

        public string Feed { get; set; }

        public string Url { get; set; }

        public int Variable { get; set; }

        public int Server { get; set; }

        public string Folder { get; set; }

        public string ImageType { get; set; }

        public string FullUrl { get; set; }

        public int Blog { get; set; }

        public string Ss_folder { get; set; }

        public string Display_Name { get; set; }
    }
}