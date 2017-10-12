namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration
{
    public static class HangfireQueueConfiguration
    {
        public static string HighPriority
        {
            get
            {
                return "high_priority";
            }
        }

        public static string NormalPriority
        {
            get
            {
                return "normal_priority";
            }
        }

        public static string Default
        {
            get
            {
                return "default";
            }
        }
    }
}
