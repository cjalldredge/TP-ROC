namespace RemoteOBSController
{
    public class PairRequest
    {
        public string type { get; set; }
        public string id { get; set; }

        public PairRequest(string _type, string _id)
        {
            type = _type;
            id = _id;
        }
    }
}
