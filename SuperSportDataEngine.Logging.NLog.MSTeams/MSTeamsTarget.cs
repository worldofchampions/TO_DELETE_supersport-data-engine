using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Net;
using System.Text;

namespace SuperSportDataEngine.Logging.NLog.MSTeams
{
    [Target("MSTeamsTarget")]
    public sealed class MSTeamsTarget : TargetWithLayout
    {
        [RequiredParameter]
        public string Host { get; set; }

        public MSTeamsTarget()
        {
            Host = "localhost";
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);

            SendTheMessageToRemoteHost(Host, logEvent.Message, logEvent?.Level?.Name);
        }

        private void SendTheMessageToRemoteHost(string host, string message, string level)
        {
            try
            {
                bool isErrorLevel = level == "Error" || level == "Fatal";
                string themeHexCode = isErrorLevel ? "FF0000" : "0072C6";

                string jsonPostData = "{ " +
                    "\"@context\" : \"https://schema.org/extensions/\", " +
                    "\"@type\": \"MessageCard\", " +
                    "\"themeColor\": \"" + themeHexCode + "\", " +
                    "\"title\": \"Level: "+ level +"\", " +
                    "\"text\": \"" + message.ToString() + "\"}";

            
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    client.UploadString(Host, "POST", jsonPostData);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
