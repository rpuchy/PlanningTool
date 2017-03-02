using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    public class ObjectDetails
    {
        public List<ParameterDetails> Parameters { get; set; }
        public List<ObjectDetails> SubObjects { get; set; }
        public List<TableDetails> Tables { get; set; }
        public List<String> ValueTypes { get; set; } 
        public String NodeName { get; set; }
        public int minOccurs { get; set; }
        public int maxOccurs { get; set; }
        public string desc { get; set; }
        public XmlNode XmlSnippet { get; set; }

        public static ObjectDetails LoadFromXml(XmlNode node)
        {
            ObjectDetails temp = new ObjectDetails();
            temp.Parameters = Schema.GetParametersFromXml(node, Schema.Classifier.All);
            temp.NodeName = node.Name;
            temp.minOccurs = int.Parse(node.Attributes[Schema.minOccurs]?.Value);
            temp.maxOccurs = node.Attributes[Schema.maxOccurs]?.Value == "" ? 9999 : int.Parse(node.Attributes[Schema.maxOccurs]?.Value);
            temp.SubObjects = Schema.GetObjectsFromXml(node, Schema.Classifier.All);
            temp.ValueTypes = Schema.GetValueTypesfromXml(node);            
            temp.XmlSnippet = node.CloneNode(true);            
            return temp;
        }




    }
}
