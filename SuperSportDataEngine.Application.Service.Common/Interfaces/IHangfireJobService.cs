using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.Common.Interfaces
{
    public interface IHangfireJobService
    {
        Task IngestPastFixturesNotIngestedYet(CancellationToken cancellationToken);
    }
}
