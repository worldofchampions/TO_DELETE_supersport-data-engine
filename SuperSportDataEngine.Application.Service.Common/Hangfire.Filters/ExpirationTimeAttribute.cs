using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Filters
{
    public class ExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(2);
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(2);
        }
    }
}
