using System.Collections.Generic;

namespace RemoteOBSController
{
    class ScenesListTP
    {
        public string type { get; set; }
        public string id { get; set; }
        public List<string> value { get; set; }

        //public string instanceId { get; set; }

        public ScenesListTP(string _type, string _id, List<string> _value, string _instanceId)
        {
            type = _type;
            id = _id;
            value = _value;
            //instanceId = _instanceId;
        }

       public ScenesListTP(string _type, string _id, List<string> _value)
        {
            type = _type;
            id = _id;
            value = _value;
        }
    }
}
