using SuperSportDataEngine.Application.Service.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.Common.Services
{
    public class SystemTimeService : ISystemTimeService
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public DateTimeOffset GetDateTimeOffsetNow()
        {
            return DateTimeOffset.Now;
        }

        public DateTimeOffset GetDateTimeOffsetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }

        public DateTime GetDateTimeUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
