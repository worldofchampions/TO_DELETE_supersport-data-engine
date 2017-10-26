namespace SuperSportDataEngine.Repository.EntityFramework.Common.Database
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public static class TimestampHelper
    {
        public static void ApplyTimestamps(DbContext dbContext)
        {
            var entities = dbContext.ChangeTracker.Entries()
                .Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var now = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseModel)entity.Entity).TimestampCreated = now;
                }

                ((BaseModel)entity.Entity).TimestampUpdated = now;
            }
        }
    }
}