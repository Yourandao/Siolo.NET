using System.Collections.Generic;

using Newtonsoft.Json;
using System.Linq;

using Siolo.NET.Components.Neo4j;

namespace Siolo.NET.Components.Logstash
{
    public class EventIncident : EventDrop
    {
        public string RestrictingPolicy { get; set; }
        public string[][] PossibleRoutes { get; set; }

        public EventIncident() :
           base()
        {
            event_type = "incident";
        }

        public EventIncident(string ip, string md5, string fullClass, string restrictingPolicy) :
           base(ip, md5, fullClass)
        {
            RestrictingPolicy = restrictingPolicy;
            event_type = "incident";
        }

        public void SetPossibleRoutes(List<List<Neo4jRelation>> pathArray)
        {
            PossibleRoutes = pathArray.Select(path => path.Select(n => n.ip).ToArray()).ToArray();
        }
    }
}
