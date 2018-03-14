using System.Linq;
using System.Reflection;
using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public class AutoMapperConfig
    {
        public static void InitializeMappings()
        {
            // Get all the mapping profiles from the current assembly.
            var types = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => 
                                t.BaseType == typeof(Profile));

            // Add all the mapping 
            // profiles to Automapper.
            Mapper.Initialize(cfg =>
            {
                foreach (var type in types)
                    cfg.AddProfile(type);
            });
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}