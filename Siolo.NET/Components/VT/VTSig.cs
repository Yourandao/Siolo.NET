using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siolo.NET.Components.VT
{
   public class VTSigRootobject
   {
      public VTSig[] sig_array { get; set; }
   }

   public class VTSig
   {
      public string type { get; set; }
      public string extension { get; set; }
      public string mime { get; set; }
      public int offset { get; set; }
      public string[] signature { get; set; }
   }
}
