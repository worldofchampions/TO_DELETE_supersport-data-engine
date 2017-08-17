namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading.Tasks;

    // TODO: [Davide] Temporary example reference code for team (delete this later).
    public interface ITemporaryExampleService
    {
        string HelloMessage();

        Task SqlDatabaseTemporaryExampleInsertData();

        Task SqlDatabaseTemporaryExampleQueryData();
    }
}