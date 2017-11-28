namespace SuperSportDataEngine.Repository.EntityFramework.Common.Database
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    public static class DatabaseInitializerHelper
    {
        public static void AutoApplyMigrations(DbMigrator dbMigrator)
        {
            ApplyForwardPendingMigrations(dbMigrator);
            ApplyRollBackMigrations(dbMigrator);
        }

        private static void ApplyForwardPendingMigrations(DbMigrator dbMigrator)
        {
            // Apply forward migrations and seeding database regardless
            // of whether there are pending migrations or not.
            // This is so that new seed data is applied
            // even though there are no pending migrations.
            //if (dbMigrator.GetPendingMigrations().Any())
                dbMigrator.Update();
        }

        private static void ApplyRollBackMigrations(DbMigrator dbMigrator)
        {
            var latestMigrationAppliedToDatabase = dbMigrator.GetDatabaseMigrations().OrderBy(x => x).Last();
            var latestMigrationDefinedInCode = dbMigrator.GetLocalMigrations().OrderBy(x => x).Last();

            if (latestMigrationAppliedToDatabase.Equals(latestMigrationDefinedInCode))
                return;

            dbMigrator.Configuration.AutomaticMigrationDataLossAllowed = true;
            dbMigrator.Update(latestMigrationDefinedInCode);
        }
    }
}