using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy
{
    public class LegacyAuthFeedConsumerEntity
    {
        public List<LegacyAccessItemEntity> AccessItems { get; set; }
        public bool Active { get; set; }
        public bool AllowAll { get; set; }
        public string AuthKey { get; set; }
        public int Id { get; set; }
        public List<LegacyMethodAccessEntity> MethodAccess { get; set; }
        public string Name { get; set; }
    }

    public class LegacyAccessItemEntity
    {
        public int Id { get; set; }
        public string MethodAccess { get; set; }
        public string Sport { get; set; }
        public string Tournament { get; set; }
    }

    public class LegacyMethodAccessEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}