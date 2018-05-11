using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyVenueEntity
    {
        public Guid Id { get; set; }
        public int ProviderVenueId { get; set; }
        public string Name { get; set; }
        public string NameCmsOverride { get; set; }
    }
}
