namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    // TODO: [Davide] Temporary example reference code for team (delete this later).
    public class TemporaryExampleService : ITemporaryExampleService
    {
        private readonly IBaseEntityFrameworkRepository<Player> _playerRepository;
        private readonly IBaseEntityFrameworkRepository<Sport> _sportRepository;
        private readonly IBaseEntityFrameworkRepository<SportTournament> _sportTournamentRepository;
        private readonly ITemporaryExampleMongoDbRepository _temporaryExampleMongoDbRepository;

        public TemporaryExampleService(
            IBaseEntityFrameworkRepository<Player> playerRepository,
            IBaseEntityFrameworkRepository<Sport> sportRepository,
            IBaseEntityFrameworkRepository<SportTournament> sportTournamentRepository,
            ITemporaryExampleMongoDbRepository temporaryExampleMongoDbRepository)
        {
            _playerRepository = playerRepository;
            _sportRepository = sportRepository;
            _sportTournamentRepository = sportTournamentRepository;
            _temporaryExampleMongoDbRepository = temporaryExampleMongoDbRepository;
        }

        public string HelloMessage()
        {
            return "Hello from: (SuperSportDataEngine.ApplicationLogic.Services).TemporaryExampleService"
                + Environment.NewLine + _temporaryExampleMongoDbRepository.HelloMessage();
        }

        public async Task SqlDatabaseTemporaryExampleInsertData()
        {
            try
            {
                var sport1 = new Sport
                {
                    Name = "Tennis (Temp)"
                };

                var player11 = new Player
                {
                    Sport = sport1,
                    FirstName = "Andre",
                    LastName = "Agassi",
                    DisplayName = "Andre Agassi (Temp)"
                };

                var player12 = new Player
                {
                    Sport = sport1,
                    FirstName = "Maria",
                    LastName = "Sharapova",
                    DisplayName = "Maria Sharapova (Temp)"
                };

                var player13 = new Player
                {
                    //Sport = sport1,
                    FirstName = "Novak",
                    LastName = "Djokovic",
                    DisplayName = "Novak Djokovic (Temp unassigned)"
                };

                var sport2 = new Sport
                {
                    Name = "Cricket (Temp)"
                };

                var player21 = new Player
                {
                    Sport = sport2,
                    FirstName = "Sachin",
                    LastName = "Tendulkar",
                    DisplayName = "Sachin Tendulkar (Temp)"
                };

                var player22 = new Player
                {
                    Sport = sport2,
                    FirstName = "AB",
                    LastName = "de Villiers",
                    DisplayName = "AB de Villiers (Temp)"
                };

                var player23 = new Player
                {
                    //Sport = sport2,
                    FirstName = "Hashim",
                    LastName = "Amla",
                    DisplayName = "Hashim Amla (Temp unassigned)"
                };

                var sport3 = new Sport
                {
                    Name = "Football (Temp unassigned)"
                };

                _sportRepository.Add(sport1);
                _sportRepository.Add(sport2);
                _sportRepository.Add(sport3);
                await _sportRepository.SaveAsync();

                _playerRepository.Add(player11);
                _playerRepository.Add(player12);
                _playerRepository.Add(player13);
                _playerRepository.Add(player21);
                _playerRepository.Add(player22);
                _playerRepository.Add(player23);
                await _playerRepository.SaveAsync();

                var sportTournament1 = new SportTournament
                {
                    TournamentName = "Tennis tournament 1 (Temp enabled)",
                    IsEnabled = true
                };

                var sportTournament2 = new SportTournament
                {
                    TournamentName = "Tennis tournament 2 (Temp disabled)",
                    IsEnabled = false
                };

                _sportTournamentRepository.Add(sportTournament1);
                _sportTournamentRepository.Add(sportTournament2);
                await _sportTournamentRepository.SaveAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }

        public async Task SqlDatabaseTemporaryExampleQueryData()
        {
            try
            {
                var allSports = _sportRepository.All().ToList();

                var allPlayersOrderedByName = _playerRepository.All().OrderBy(x => x.DisplayName).ToList();

                var allPlayersLastNameSharapova = (from x in _playerRepository
                                                   where x.LastName == "Sharapova"
                                                   select x).ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }
    }
}