using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROCClient.common.messages
{
    class ROCServers
    {
        public string type { get; set; }
        public string id { get; set; }
        public List<string> connections { get; set; }
    }
}
