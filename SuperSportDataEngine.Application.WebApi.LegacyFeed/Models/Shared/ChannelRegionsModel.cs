using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class ChannelRegions
    {        
        public string tvchannel { get; set; }
        public string regions { get; set; }
        public string tvOptionSelected { get; set; }
    }


}