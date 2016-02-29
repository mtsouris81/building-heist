using System.IO;
using System.Xml.Serialization;

namespace Weenus.Network
{
    public class XmlWebServiceCall<T> : AbstractSerializedWebServiceCall<T>
    {
        public XmlWebServiceCall(string name) : base(name)
        {
        }
        public override Tobj Deserialize<Tobj>(string xml)
        {
            XmlSerializer x = new XmlSerializer(typeof(Tobj));
            using (StringReader reader = new StringReader(xml))
            {
                return (Tobj)x.Deserialize(reader);
            }
        }
        public override string Serialize<Tobj>(Tobj data)
        {
            string result = null;
            XmlSerializer x = new XmlSerializer(typeof(Tobj));
            using (StringWriter writer = new StringWriter())
            {
                x.Serialize(writer, data);
                result = x.ToString();
            }
            return result;
        }
    }
}
