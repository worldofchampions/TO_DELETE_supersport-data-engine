using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{
    public static class HangfireConfiguration
    {
        private static SqlServerStorageOptions Options;

        private static SqlServerStorageOptions Get_options()
        {
            if (Options != null)
            {
                return Options;
            }

            Options = 
                new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                };

            return Options;
        }

        private static string ConnectionString;

        private static string Get_connectionString()
        {
            if(!string.IsNullOrEmpty(ConnectionString))
            {
                return ConnectionString;
            }

            ConnectionString = 
                ConfigurationManager
                    .ConnectionStrings["SqlDatabase_Hangfire"]
                    .ConnectionString;

            return ConnectionString;
        }

        private static JobStorage JobStorage;

        private static JobStorage Get_jobStorage()
        {
            if(JobStorage != null)
            {
                return JobStorage;
            }

            JobStorage = 
                new SqlServerStorage(
                    Get_connectionString(),
                    Get_options());

            return JobStorage;
        }
            
        
        public static JobStorage JOB_STORAGE { get => Get_jobStorage(); }
    }
}
