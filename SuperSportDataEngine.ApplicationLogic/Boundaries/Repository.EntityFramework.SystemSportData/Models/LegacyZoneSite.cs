using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    public class LegacyZoneSite
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
