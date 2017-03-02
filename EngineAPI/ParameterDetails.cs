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
            t.Name = param.Name;
            t.Description = param.Attributes[Schema.desc]?.Value;
            t.Type = param.Attributes[Schema.type]?.Value;
            t.UniqueScopeTo = param.Attributes[Schema.UniqueScopeTo]?.Value;
            t.bounds = param.Attributes[Schema.bounds]?.Value;
            t.constraint = param.Attributes[Schema.constraint]?.Value;
            t.defaultType = param.Attributes[Schema.defaultType]?.Value;
            t.defaultval = param.Attributes[Schema.defaultValue]?.Value;
            t.depends = param.Attributes[Schema.depends]?.Value;
            t.isUniqueScope = param.Attributes[Schema.isUniqueScope]?.Value == "true";
            t.minOccurs = int.Parse(param.Attributes[Schema.minOccurs]?.Value);
            t.maxOccurs = param.Attributes[Schema.maxOccurs]?.Value == "" ? 9999 : int.Parse(param.Attributes[Schema.maxOccurs]?.Value);
            t.metaType = param.Attributes[Schema.metaType]?.Value;
            t.reference = param.Attributes[Schema.reference]?.Value;
            t.referenceField = param.Attributes[Schema.referenceField]?.Value;
            t.referenceMetaType = param.Attributes[Schema.referenceMetaType]?.Value;
            t.referenceSubType = param.Attributes[Schema.referenceSubType]?.Value;
            t.subType = param.Attributes[Schema.subType]?.Value;
            t.unique = param.Attributes[Schema.unique]?.Value;
            t.uniqueSearchRef = param.Attributes[Schema.uniqueSearchRef]?.Value;
            return t;
        }



    }
}
