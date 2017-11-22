namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SystemSportDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SystemSportDataContext context)
        {
            context.LegacyAuthFeedConsumers.AddOrUpdate(
              p => p.Name,
              new LegacyAuthFeedConsumer
              {
                  Name = "iMMedia",
                  Active = true,
                  AllowAll = true,
                  AuthKey = "A22C8A7F-844C-4E19-97FB-0A63A71BA290",
                  MethodAccess = new HashSet<LegacyMethodAccess>()
                  {
                          new LegacyMethodAccess
                          {
                              Name = "MATCHDETAILS"
                          },
                          new LegacyMethodAccess
                          {
                              Name = "VIDEO"
                          },
                          new LegacyMethodAccess
                          {
                              Name = "LIVE"
                          }
                  }
              }
            );
        }
    }
}