using System.Threading;

namespace SuperSportDataEngine.Common
{
    // TODO: [Davide] A temporary placeholder project with references linked. (Delete this later later if not needed after implementing data from 2+ different providers).
    public static class AccessControl
    {
        public static SemaphoreSlim PublicSportsData_SeasonRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_FixturesRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_PlayersRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_TeamsRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_FlatLogsRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_TournamentRepo_Access = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim PublicSportsData_LogGroupsRepo_Access = new SemaphoreSlim(1, 1);
    }
}