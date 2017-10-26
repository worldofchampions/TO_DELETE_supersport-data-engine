namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context
{
    using SuperSportDataEngine.Repository.EntityFramework.Common.Database;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Migrations;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    public class PublicSportDatabaseInitializer : IDatabaseInitializer<DbContext>
    {
        public void InitializeDatabase(DbContext context)
        {
            var dbMigrator = new DbMigrator(new Configuration());
            DatabaseInitializerHelper.AutoApplyMigrations(dbMigrator);
        }
    }
}