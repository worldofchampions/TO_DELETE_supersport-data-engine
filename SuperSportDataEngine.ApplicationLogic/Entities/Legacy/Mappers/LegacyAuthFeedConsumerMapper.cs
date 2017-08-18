using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System.Collections.Generic;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers
{
    public static class LegacyAuthFeedConsumerMapper
    {
        public static LegacyAuthFeedConsumer MapToModel(LegacyAuthFeedConsumerEntity model)
        {
            var acceItems = model.AccessItems.Select(c => MapTo(c));
            var methodAccessItems = model.MethodAccess.Select(c => MapTo(c));
            return new LegacyAuthFeedConsumer
            {
                Id = model.Id,
                AccessItems = new HashSet<LegacyAccessItem>(acceItems),
                Active = model.Active,
                AllowAll = model.AllowAll,
                AuthKey = model.AuthKey,
                MethodAccess = new HashSet<LegacyMethodAccess>(methodAccessItems),
                Name = model.Name
                
            };
        }

        private static LegacyAccessItem MapTo(LegacyAccessItemEntity model)
        {
            return new LegacyAccessItem
            {
                MethodAccess = model.MethodAccess,
                Sport = model.Sport,
                Tournament = model.Tournament
            };
        }

        private static LegacyMethodAccess MapTo(LegacyMethodAccessEntity model)
        {
            return new LegacyMethodAccess
            {
                Id = model.Id,
                Name = model.Name
            };
        }
    }
}