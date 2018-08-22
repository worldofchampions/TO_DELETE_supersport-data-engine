using Hangfire;
using Hangfire.SqlServer;
using System;
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
                if (_storageOptions == null)
                {
                    _storageOptions =
                        new SqlServerStorageOptions
                        {
                            PrepareSchemaIfNecessary = true
                        };
                }

                return _storageOptions;
            }
        }

        // Hangfire SQL Server connection string.
        private static string _connectionString;

        private static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString =
                        ConfigurationManager
                            .ConnectionStrings["SqlDatabase_Hangfire"]
                            .ConnectionString;
                }

                return _connectionString;
            }
        }

        // Hangfire SQL Server storage.
        private static JobStorage _jobStorage;

        public static JobStorage JobStorage
        {
            get
            {
                if (_jobStorage == null)
                {
                    _jobStorage =
                        new SqlServerStorage(
                            ConnectionString,
                            StorageOptions);
                }

                return _jobStorage;
            }
        }

        // Hangfire Queues options.
        private static BackgroundJobServerOptions _jobServerOptions;

        public static BackgroundJobServerOptions JobServerOptions
        {
            get
            {
                if (_jobServerOptions == null)
                {
                    _jobServerOptions =
                        new BackgroundJobServerOptions
                        {
                            WorkerCount =
                                Environment.ProcessorCount *
                                Convert.ToInt32(ConfigurationManager.AppSettings["WorkerCountMultiplier"]),
                            Queues =
                                new[] {
                                    // The default queue is added first because
                                    // Hangfire places jobs in this queue when it gets retried.
                                    // This will be the highest priority queue.
                                    // In code, no jobs will be scheduled on this queue.
                                    HangfireQueueConfiguration.HighPriority,
                                    HangfireQueueConfiguration.NormalPriority,
                                    HangfireQueueConfiguration.DefaultPriority
                                }
                        };
                }

                return _jobServerOptions;
            }
        }
    }
}
