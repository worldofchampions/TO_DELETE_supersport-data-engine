using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models

{
    public class RugbyResult
    {
        public RugbyFixture Fixture { get; set; }

        public int AwayTeamScore { get; set; }
        public int HomeTeamScore { get; set; }

        public bool LiveScored { get; set; }

    }
}
