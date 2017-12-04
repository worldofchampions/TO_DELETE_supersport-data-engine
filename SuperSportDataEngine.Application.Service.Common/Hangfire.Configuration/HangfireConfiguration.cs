using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Configuration;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{

    public static class HangfireConfiguration
    {
        static HangfireConfiguration()
        {
            ConnectionString =
                ConfigurationManager
                    .ConnectionStrings["SqlDatabase_Hangfire"]
                    .ConnectionString;

            StorageOptions =
                new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                };

            JobStorage =
                new SqlServerStorage(
                    ConnectionString,
                    StorageOptions);

            JobServerOptions =
                new BackgroundJobServerOptions
                {
                    WorkerCount =
                        Environment.ProcessorCount *
                        Convert.ToInt32(ConfigurationManager.AppSettings["WorkerCountMultiplier"]),
                    Queues =
                        new[] {
                            HangfireQueueConfiguration.HighPriority,
                            HangfireQueueConfiguration.NormalPriority }
                };
        }

        private static SqlServerStorageOptions StorageOptions { get; }
        
        private static string ConnectionString { get; }
        
        public static JobStorage JobStorage { get; }
        
        public static BackgroundJobServerOptions JobServerOptions { get; }
    }
}
