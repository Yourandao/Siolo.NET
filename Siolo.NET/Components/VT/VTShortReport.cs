using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.VT
{
   public class VTShortReport
   {
      public string  md5               { get; set; }
      public int     file_size         { get; set; }
      public string  full_class        { get; set; }
      public string  detection_engine  { get; set; } = "Kaspersky";
      public string  report_date       { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
      public string  report_time       { get; set; } = DateTime.Now.ToString("HH:mm:ss");
   }
}
