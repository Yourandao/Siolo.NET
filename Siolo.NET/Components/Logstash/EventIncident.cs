using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Logstash
{
   public class EventIncident : EventDrop
   {
      public string[][] possible_routes { get; set; }

      public EventIncident() :
         base()
      {
         event_type = "incident";
      }

      public EventIncident(string src_ip, string md5, string full_class) :
         base(src_ip, md5, full_class)
      {
         event_type = "incident";
      }

      public void SetPossibleRoutes(PathArc[][] path_array)
      {
         possible_routes = (from path in path_array
                            select (new string[] { path.First().@from })
                              .Concat(from arc in path select arc.to).ToArray()).ToArray();
      }

      public void SetPossibleRoutes(string path_array_json)
      {
         SetPossibleRoutes(JsonConvert.DeserializeObject<PathArc[][]>(path_array_json));
      }
   }
}
