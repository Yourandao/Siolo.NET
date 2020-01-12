using System.Collections.Generic;

using System.Linq;

using Siolo.NET.Components.Neo4j;
using Siolo.NET.Components.Network;
using Siolo.NET.Components.VT;

namespace Siolo.NET.Components.Logstash
{
    public class EventIncident : EventDrop
    {
        public string RestrictingPolicy { get; set; }
        public string[][] PossibleRoutes { get; set; }

        public EventIncident()  => EventType = "incident";

        public EventIncident(string ip, string md5, string fullClass, string restrictingPolicy) :
           base(ip, md5, fullClass)
        {
            RestrictingPolicy = restrictingPolicy;
            EventType = "incident";
        }

        public void SetPossibleRoutes(IEnumerable<List<NodeEntity>> pathArray)
        {
            PossibleRoutes = pathArray.Select(path => path.Select(n => n.ip).ToArray()).ToArray();
        }

        public void ExcludeRestrictedRoutes(VTShortReport shortReport, Redis redis)
        {
	        PossibleRoutes = (from route in PossibleRoutes
	                            where route.Skip(1).Take(route.Length - 2)
	                                       .All(host => NetworkUtility.IsSubnet(host) || 
	                                                    !NetworkUtility.IsRestricted(redis.GetHostWildcards(host), 
	                                                                                 shortReport.full_class.ToLower()).Result)
	                            select route).ToArray();
        }
    }
}
