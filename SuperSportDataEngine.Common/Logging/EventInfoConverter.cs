using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NLog;

namespace SuperSportDataEngine.Logging.NLog.Logging
{
    public class EventInfoConverter : CustomCreationConverter<LogEventInfo>
    {
        private string _level { get; set; }

        public EventInfoConverter(string s)
        {
            JToken eventinfo = JObject.Parse(s);
            var childs = eventinfo.Children();
            foreach (var item in childs)
            {
                if (((JProperty)item).Name == "Level")
                {
                    var m = ((JProperty)item).Value.Children();
                    foreach (var item1 in m)
                    {
                        _level = ((JProperty)item1).Value.ToString();
                        break;
                    }

                    break;
                }
            }
        }

        public override LogEventInfo Create(Type objectType)
        {
            LogEventInfo eventInfo = new LogEventInfo();
            switch (_level)
            {
                case "Info":
                    eventInfo = new LogEventInfo(LogLevel.Info, "", "");
                    break;
                case "Debug":
                    eventInfo = new LogEventInfo(LogLevel.Debug, "", "");
                    break;
                case "Error":
                    eventInfo = new LogEventInfo(LogLevel.Error, "", "");
                    break;
                case "Warn":
                    eventInfo = new LogEventInfo(LogLevel.Warn, "", "");
                    break;
            }
            return eventInfo;
        }
    }
}
