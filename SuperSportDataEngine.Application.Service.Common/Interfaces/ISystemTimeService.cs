using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.Common.Interfaces
{
    public interface ISystemTimeService
    {
        DateTimeOffset GetDateTimeOffsetNow();
        DateTimeOffset GetDateTimeOffsetUtcNow();
        DateTime GetDateTimeNow();
        DateTime GetDateTimeUtcNow();
    }
}
