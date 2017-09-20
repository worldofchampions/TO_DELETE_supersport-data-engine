namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class RugbyResult
    {
        public virtual RugbyFixture Fixture { get; set; }

        public int HomeTeamScore { get; set; }

        public int AwayTeamScore { get; set; }

        public bool LiveScored { get; set; }
    }
}