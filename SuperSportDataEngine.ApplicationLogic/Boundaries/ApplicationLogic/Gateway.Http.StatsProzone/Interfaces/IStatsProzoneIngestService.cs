using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IStatsProzoneIngestService
    {
        Task<string> GetReferenceData();
    }
}
