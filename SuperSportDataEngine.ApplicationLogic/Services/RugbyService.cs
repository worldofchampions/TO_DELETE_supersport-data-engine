using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly IBaseEntityFrameworkRepository<Log> _logRepository;

        public RugbyService(IBaseEntityFrameworkRepository<Log> logRepository)
        {
            _logRepository = logRepository;
        }
        public IEnumerable<LogEntity> GetLogs()
        {
            var logs = _logRepository.All().Select(log => Mapper.Map<LogEntity>(log));
            return logs;
        }
    }
}