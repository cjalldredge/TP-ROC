/// <summary>
/// Author: Cameron Alldredge
/// Date: May 23, 2020
/// Description:
///     Generic object to format choice updates sent to TouchPortal
/// </summary>

using System.Collections.Generic;

namespace ROCClient
{
    class ChangeList
    {
        public string type { get; set; }
        public string id { get; set; }
        public List<string> options { get; set; }
    }
}
