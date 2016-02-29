namespace Weenus.Network
{
    public class JsonWebServiceCall<T> : AbstractSerializedWebServiceCall<T>
    {
        public JsonWebServiceCall(string name) : base(name)
        {
        }
        public override Tobj Deserialize<Tobj>(string xml)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tobj>(xml);
        }
        public override string Serialize<Tobj>(Tobj data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
    }
}
