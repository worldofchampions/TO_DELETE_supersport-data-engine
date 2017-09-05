using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Configuration;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Filters
{
    public class ExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private static int _jobExpirationTimeoutInDays;

        public ExpirationTimeAttribute()
        {
            _jobExpirationTimeoutInDays = Convert.ToInt32(ConfigurationManager.AppSettings["JobExpirationTimeoutInDays"]);
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(_jobExpirationTimeoutInDays);
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(_jobExpirationTimeoutInDays);
        }
    }
}
