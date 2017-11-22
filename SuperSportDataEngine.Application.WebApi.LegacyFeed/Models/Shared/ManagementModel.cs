using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class ManagementModel:PersonModel
    {
        public string ManagementType { get; set; }
    }
}
