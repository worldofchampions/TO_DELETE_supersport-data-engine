namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using System.Collections.Generic;

    public class LegacyAuthFeedConsumer : BaseModel
    {
        public ISet<LegacyAccessItem> AccessItems { get; set; }

        public bool Active { get; set; }

        public bool AllowAll { get; set; }

        public string AuthKey { get; set; }

        public int Id { get; set; }

        public ISet<LegacyMethodAccess> MethodAccess { get; set; }

        public string Name { get; set; }
    }

    public class LegacyAccessItem : BaseModel
    {
        public int Id { get; set; }

        public string MethodAccess { get; set; }

        public string Sport { get; set; }

        public string Tournament { get; set; }
    }

    public class LegacyMethodAccess : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}