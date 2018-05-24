using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Services.Cms
{
    public class MotorsportCmsService : IMotorsportCmsService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        IMapper iMapper;

        public MotorsportCmsService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;

            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<MotorsportLeague, MotorsportLeagueEntity>();
                });
            iMapper = config.CreateMapper();
        }

        public async Task<PagedResultsEntity<MotorsportLeagueEntity>> GetAllLeagues(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var leagues = (PagedResultsEntity<MotorsportLeagueEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                leagues = await CreatePagedResults<MotorsportLeague, MotorsportLeagueEntity>(
                                    _publicSportDataUnitOfWork.MotorsportLeagues.Where(cond => cond.IsEnabled).Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)).OrderBy(league => league.Slug), pageIndex, pageSize, abpath, query);
            }
            else
            {
                leagues = await CreatePagedResults<MotorsportLeague, MotorsportLeagueEntity>(
                                            _publicSportDataUnitOfWork.MotorsportLeagues.Where(cond => cond.IsEnabled).OrderBy(league => league.Slug), pageIndex, pageSize, abpath, query);
            }

            if (leagues.Results.Any())
            {
                return leagues;
            }
            return null;
        }

        public async Task<MotorsportLeagueEntity> GetLeagueById(Guid id)
        {
            var motorsportLeague = await Task.FromResult(_publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(
                                                        league => league.Id == id));

            if (motorsportLeague != null)
            {
                return iMapper.Map<MotorsportLeague, MotorsportLeagueEntity>(motorsportLeague);
            }

            return null;
        }

        protected async Task<PagedResultsEntity<TReturn>> CreatePagedResults<T, TReturn>(IEnumerable<T> queryable, int pageIndex, int pageSize, string abpath, string query)
        {
            var skipAmount = pageSize * (pageIndex - 1);

            var projection = queryable.Skip(skipAmount).Take(pageSize).ToList();
            var totalNumberOfRecords = await Task.FromResult(queryable.Count());

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            var nextPageUrl = pageIndex == totalPageCount ? null : abpath + string.Format("?pageIndex={0}&pageSize={1}", pageIndex + 1, pageSize)
                                                    + (!String.IsNullOrEmpty(query) ? "&query=" + query : "");

            var results = iMapper.Map<IEnumerable<T>, IEnumerable<TReturn>>(projection);
            return new PagedResultsEntity<TReturn>
            {
                Results = results,
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords,
                NextPageUrl = nextPageUrl
            };
        }
    }
}
