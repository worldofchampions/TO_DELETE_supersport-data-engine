namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context
{
    using SuperSportDataEngine.Repository.EntityFramework.Common.Database;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    public class SystemSportDatabaseInitializer : IDatabaseInitializer<DbContext>
    {
        public void InitializeDatabase(DbContext context)
        {
            var dbMigrator = new DbMigrator(new Configuration());
            DatabaseInitializerHelper.AutoApplyMigrations(dbMigrator);
        }
    }
}