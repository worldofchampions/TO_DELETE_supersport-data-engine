namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class SchedulerDashboardUser : BaseModel
    {
        public string Username { get; set; }

        public string PasswordPlain { get; set; }
    }
}