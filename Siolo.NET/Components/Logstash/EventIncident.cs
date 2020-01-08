using Newtonsoft.Json;
using System.Linq;

namespace Siolo.NET.Components.Logstash
{
    public class EventIncident : EventDrop
    {
        public string[][] PossibleRoutes { get; set; }

        public EventIncident() :
           base()
        {
            event_type = "incident";
        }

        public EventIncident(string ip, string md5, string fullClass) :
           base(ip, md5, fullClass)
        {
            event_type = "incident";
        }

        private void SetPossibleRoutes(PathArc[][] pathArray)
        {
            PossibleRoutes = (from path in pathArray
                               select (new[] { path.First().@from })
                                 .Concat(from arc in path select arc.to).ToArray()).ToArray();
        }

        public void SetPossibleRoutes(string pathArrayJson)
        {
            SetPossibleRoutes(JsonConvert.DeserializeObject<PathArc[][]>(pathArrayJson));
        }
    }
}
