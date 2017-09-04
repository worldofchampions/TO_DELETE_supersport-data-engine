using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{

    public static class HangfireConfiguration
    {
        // Hangfire SQL Server Options.
        private static SqlServerStorageOptions _storageOptions;

        private static SqlServerStorageOptions StorageOptions
        {
            get
            {
                if (_storageOptions != null)
                {
                    return _storageOptions;
                }

                _storageOptions =
                    new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true
                    };

                return _storageOptions;
            }
        }

        // Hangfire SQL Server connection string.
        private static string _connectionString;

        private static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return _connectionString;
                }

                _connectionString =
                    ConfigurationManager
                        .ConnectionStrings["SqlDatabase_Hangfire"]
                        .ConnectionString;

                return _connectionString;
            }
        }

        // Hangfire SQL Server storage.
        private static JobStorage _jobStorage;

        public static JobStorage JobStorage
        {
            get
            {
                if (_jobStorage != null)
                {
                    return _jobStorage;
                }

                _jobStorage =
                    new SqlServerStorage(
                        ConnectionString,
                        StorageOptions);

                return _jobStorage;
            }
        }

        // Hangfire Queues options.
        private static BackgroundJobServerOptions _jobServerOptions;

        public static BackgroundJobServerOptions JobServerOptions
        {
            get
            {
                if (_jobServerOptions != null)
                {
                    return _jobServerOptions;
                }

                _jobServerOptions =
                    new BackgroundJobServerOptions
                    {
                        Queues = 
                            new[] {
                                HangfireQueueConfiguration.HighPriority,
                                HangfireQueueConfiguration.NormalPriority }
                    };

                return _jobServerOptions;
            }
        }
    }
}
