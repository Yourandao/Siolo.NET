using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.Logstash
{
   public class EventDrop : EventBase
   {
      public string md5          { get; set; }
      public string full_class   { get; set; }

      public EventDrop()
      {
         event_type = "drop";
      }

      public EventDrop(string src_ip, string md5, string full_class)
      {
         event_type = "drop";
         this.src_ip = src_ip;
         this.md5 = md5;
         this.full_class = full_class;
      }
   }
}
