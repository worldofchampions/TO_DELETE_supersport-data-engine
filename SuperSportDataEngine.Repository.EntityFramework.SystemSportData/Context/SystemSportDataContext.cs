namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using System.Data.Entity;

    public class SystemSportDataContext : DbContext
    {
        public SystemSportDataContext() : base("name=SqlDatabase_SystemSportData")
        {
        }

        public DbSet<SportTournament> SportTournaments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfiguration.ApplyFluentApiConfigurations(modelBuilder);
        }
    }
}