using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    public class AlternateCommentsModel
    {
        public string Language { get; set; }
        public List<MatchEventModel> Commentary { get; set; }
    }
}