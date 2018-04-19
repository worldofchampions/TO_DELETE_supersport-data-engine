using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyFixtureEntity
    {
        public Guid Id { get; set; }
        public int LegacyFixtureId { get; set; }
        public long ProviderFixtureId { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public bool TeamAIsHomeTeam { get; set; }
        public bool TeamBIsHomeTeam { get; set; }
        public int? TeamAScore { get; set; }
        public int? TeamBScore { get; set; }
        public bool IsDisabledOutbound { get; set; }
        public bool IsDisabledInbound { get; set; }
        public bool IsLiveScored { get; set; }
        public bool CmsOverrideModeIsActive { get; set; }
        public int GameTimeInSeconds { get; set; }
        public int RoundNumber { get; set; }

        public RugbyFixtureStatus RugbyFixtureStatus { get; set; }

        public RugbyTeamEntity TeamA { get; set; }
        public RugbyTeamEntity TeamB { get; set; }
    }
}