using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using System;
using System.Configuration;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{

    public static class HangfireConfiguration
    {
        // Hangfire SQL Server Options.
        private static SqlServerStorageOptions _storageOptions;

        //Hangire Dashboard Options.
        private static DashboardOptions _dashboardOptions;

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
                                    HangfireQueueConfiguration.HighPriority,
                                    HangfireQueueConfiguration.NormalPriority }
                        };
                }

                return _jobServerOptions;
            }
        }

        public static DashboardOptions DashboardOptions
        {
            get
            {
                if (_dashboardOptions is null)
                {
                    //Dashboard authorization
                    var filter = new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        // Require secure connection for dashboard
                        RequireSsl = false,

                        SslRedirect = false,
                        // Case sensitive login checking
                        LoginCaseSensitive = true,

                        // Users
                        Users = GetHangfireDashboardUsers()
                    });

                    _dashboardOptions = new DashboardOptions { AuthorizationFilters = new[] { filter } };
                }

                return _dashboardOptions;
            }
        }

        private static IEnumerable<BasicAuthAuthorizationUser> GetHangfireDashboardUsers()
        {
            //TODO: Move this to DB and Access it via Application Logic??
            return new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "Administrator-1",
                    // Password as plain text
                    PasswordClear = "test"
                }
            };
        }
    }
}
