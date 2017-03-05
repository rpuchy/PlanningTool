using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    public class ParameterDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string constraint { get; set; }
        public string depends { get; set; }
        public string subType { get; set; }
        public string metaType { get; set; }
        public string reference { get; set; }
        public string referenceField { get; set; }
        public string referenceSubType { get; set; }
        public string referenceMetaType { get; set; }
        public int minOccurs { get; set; }
        public int maxOccurs { get; set; }
        public string defaultval { get; set; }
        public string defaultType { get; set; }
        public string unique { get; set; }
        public string uniqueSearchRef { get; set; }
        public bool isUniqueScope { get; set; }
        public string UniqueScopeTo { get; set; }
        public string Description { get; set; }
        public string bounds { get; set; }

        public static ParameterDetails LoadFromXml(XmlNode param)
        {
            ParameterDetails t = new ParameterDetails();
            t.Name = (string)Schema.Getvalue(param, Schema.Value.Name);
            t.Description = (string)Schema.Getvalue(param, Schema.Value.Description);
            t.Type = (string)Schema.Getvalue(param, Schema.Value.Type);
            t.UniqueScopeTo = (string)Schema.Getvalue(param, Schema.Value.UniqueScopeTo);
            t.bounds = (string)Schema.Getvalue(param, Schema.Value.bounds);
            t.constraint = (string)Schema.Getvalue(param, Schema.Value.constraint);
            t.defaultType = (string)Schema.Getvalue(param, Schema.Value.defaultType);
            t.defaultval = (string)Schema.Getvalue(param, Schema.Value.defaultval);
            t.depends = (string)Schema.Getvalue(param, Schema.Value.depends);
            t.isUniqueScope = (bool)Schema.Getvalue(param, Schema.Value.isUniqueScope);
            t.minOccurs = (int)Schema.Getvalue(param, Schema.Value.minOccurs);
            t.maxOccurs = (int)Schema.Getvalue(param, Schema.Value.maxOccurs);
            t.metaType = (string)Schema.Getvalue(param, Schema.Value.metaType);
            t.reference = (string)Schema.Getvalue(param, Schema.Value.reference);
            t.referenceField = (string)Schema.Getvalue(param, Schema.Value.referenceField);
            t.referenceMetaType = (string)Schema.Getvalue(param, Schema.Value.referenceMetaType);
            t.referenceSubType = (string)Schema.Getvalue(param, Schema.Value.referenceSubType);
            t.subType = (string)Schema.Getvalue(param, Schema.Value.subType);
            t.unique = (string)Schema.Getvalue(param, Schema.Value.unique);
            t.uniqueSearchRef = (string)Schema.Getvalue(param, Schema.Value.uniqueSearchRef);
            return t;
        }

        public bool evalute(string val)
        {

            return true;
        }



    }
}
