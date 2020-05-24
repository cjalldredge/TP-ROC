using System;
using System.Collections.Generic;
using System.Text;

namespace ROCClient
{
    class TPAction
    {
        public List<Dictionary<string,string>> data { get; set; }
        public string pluginId { get; set; }
        public string actionId { get; set; }
        public string instanceId { get; set; }
        public string type { get; set; }
    }
}
