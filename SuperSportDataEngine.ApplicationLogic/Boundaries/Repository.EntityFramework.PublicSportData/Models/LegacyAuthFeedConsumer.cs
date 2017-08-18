using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class LegacyAuthFeedConsumer
    {
        public ISet<LegacyAccessItem> AccessItems { get; set; }
        public bool Active { get; set; }
        public bool AllowAll { get; set; }
        public string AuthKey { get; set; }
        public int Id { get; set; }
        public ISet<LegacyMethodAccess> MethodAccess { get; set; }
        public string Name { get; set; }
    }
    public class LegacyAccessItem
    {
        public int Id { get; set; }
        public string MethodAccess { get; set; }
        public string Sport { get; set; }
        public string Tournament { get; set; }
    }

    public class LegacyMethodAccess
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
