using Hangfire;
using Hangfire.SqlServer;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{
    public static class HangfireConfigurationSettings
    {
        private static SqlServerStorageOptions Options = new SqlServerStorageOptions { PrepareSchemaIfNecessary = false };

        public static JobStorage JOB_STORAGE = 
            new SqlServerStorage(
                    "Data Source=10.10.4.29;Initial Catalog=SuperSportDataEngine_Hangfire;User Id=SuperSportDataEngine;Password=G4GcN4ZkXKTHzvKS", 
                    Options
                );
    }
}
