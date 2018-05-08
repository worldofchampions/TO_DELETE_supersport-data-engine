namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    using System.Linq;
    using System.Reflection;
    using AutoMapper;

    public class AutoMapperConfig
    {
        public static void InitializeMappings()
        {
            // Get all the mapping profiles from the current assembly.
            var types = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t =>
                                t.BaseType == typeof(Profile));

            // Add all the mapping profiles to AutoMapper.
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
