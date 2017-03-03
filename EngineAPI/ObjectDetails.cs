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
        public string Type { get; set; }
        public int minOccurs { get; set; }
        public int maxOccurs { get; set; }
        public string desc { get; set; }
        public XmlNode XmlSnippet { get; set; }

        public static ObjectDetails LoadFromXml(XmlNode node)
        {
            ObjectDetails temp = new ObjectDetails();
            temp.Parameters = Schema.GetParametersFromXml(node, Schema.Classifier.All);
            foreach (var p in temp.Parameters)
            {
                if (p.Name=="Type" || p.Name=="Class")
                {
                    temp.Type = p.constraint;
                }
            }
            temp.NodeName = node.Name;
            temp.minOccurs = (int)Schema.Getvalue(node, Schema.Value.minOccurs);
            temp.maxOccurs = (int)Schema.Getvalue(node, Schema.Value.maxOccurs);
            temp.SubObjects = Schema.GetObjectsFromXml(node, Schema.Classifier.All);
            temp.ValueTypes = Schema.GetValueTypesfromXml(node);            
            temp.XmlSnippet = node.CloneNode(true);            
            return temp;
        }




    }
}
