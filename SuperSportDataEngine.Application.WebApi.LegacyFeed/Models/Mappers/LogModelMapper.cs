using SuperSportDataEngine.ApplicationLogic.Entities;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public static class LogModelMapper
    {
        public static LogModel MapToModel(LogEntity log)
        {
            return new LogModel
            {
                Batting = log.Batting,
                Bowling = log.Bowling,
                BonusPoints = log.BonusPoints,
                CombinedRank = log.CombinedRank,
                ConferenceRank = log.ConferenceRank,
                CricketBonus = log.CricketBonus,
                Drew = log.Drew,
                GroupName = log.GroupName,
                GroupShortName = log.GroupShortName,
                HomeGroup = log.HomeGroup,
                IsConference = log.IsConference,
                LeagueName = log.LeagueName,
                LogName = log.LogName,
                LossBonusPoints = log.LossBonusPoints,
                Lost = log.Lost,
                NetRunRate = log.NetRunRate,
                NoResult = log.NoResult,
                Played = log.Played,
                Points = log.Points,
                PointsAgainst = log.PointsAgainst,
                PointsDifference = log.PointsDifference,
                PointsFor = log.PointsFor,
                Position = log.Position,
                rank = log.rank,
                Sport = (SportType)log.Sport,
                Team = log.Team,
                TeamID = log.TeamID,
                TeamShortName = log.TeamShortName,
                TriesAgainst = log.TriesAgainst,
                TriesBonusPoints = log.TriesBonusPoints,
                TriesFor = log.TriesFor,
                Won = log.Won
            };
        }
    }
}