using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class CommentModel
    {
        public int order { get; set; }
        public string time { get; set; }
        public string text { get; set; }
    }
}
