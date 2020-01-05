using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Logstash
{
   public class EventBase
   {
      public string src_ip       { get; set; }
      public string event_type   { get; set; }
   }
}
