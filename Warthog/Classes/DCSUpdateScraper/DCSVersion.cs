using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes
{
    public class DCSVersion
    {
        public string Stable { get; set; }
        public string Beta { get; set; }
        public string Alpha { get; set; }

        public DateTime StableDate { get; set; }
        public DateTime BetaDate { get; set; }
        public DateTime AlphaDate { get; set; }
    }

}
