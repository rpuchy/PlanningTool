using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace EngineAPI
{
    public class Schema
    {

        #region Properties
        public static string desc = "desc";
        public static string type = "type";
        public static string UniqueScopeTo = "UniqueScopeTo";
        public static string bounds = "bounds";
        public static string constraint = "constraint";
        public static string defaultType = "defaultType";
        public static string defaultValue = "default";
        public static string depends = "depends";
        public static string isUniqueScope = "isUniqueScope";
        public static string maxOccurs = "maxOccurs";
        public static string minOccurs = "minOccurs";
        public static string metaType = "metaType";
        public static string reference = "reference";
        public static string referenceField = "referenceField";
        public static string referenceMetaType = "referenceMetaType";
        public static string referenceSubType = "referenceSubType";
        public static string subType = "subType";
        public static string unique = "unique";
        public static string uniqueSearchRef = "uniqueSearchRef";
        public static string valueTypes = "valueTypes";
        #endregion //Properties

        #region ReservedValues
        public static string container_name = "container";
        public static string scalar_name = "scalar";

        public static string int_name = "int";
        public static string double_name = "double";
        public static string bool_name = "Boolean";
        public static string string_name = "string";




        #endregion //ReservedValues


        public enum Classifier { All=1, Required=2,Optional=3 }
        public enum Value {Name=1, Description, Type, UniqueScopeTo, bounds, constraint,defaultType,defaultval,depends,isUniqueScope, minOccurs, maxOccurs, metaType, reference,referenceField, referenceMetaType,
        referenceSubType, subType, unique, uniqueSearchRef, valueTypes}


        public static string ValueToString(Value val)
        {
            switch(val)
            {
                case Value.Name: return "Name";
                case Value.Description: return desc;
                case Value.Type: return type;
                case Value.UniqueScopeTo: return UniqueScopeTo;
                case Value.bounds: return bounds;
                case Value.constraint: return constraint;
                case Value.defaultType: return defaultType;
                case Value.defaultval: return defaultValue;
                case Value.depends: return depends;
                case Value.isUniqueScope: return isUniqueScope;
                case Value.minOccurs: return minOccurs;
                case Value.maxOccurs: return maxOccurs;
                case Value.metaType: return metaType;
                case Value.reference: return reference;
                case Value.referenceField: return referenceField;
                case Value.referenceSubType: return referenceSubType;
                case Value.subType: return subType;
                case Value.unique: return unique;
                case Value.uniqueSearchRef: return uniqueSearchRef;
                case Value.referenceMetaType:return referenceMetaType;
                case Value.valueTypes: return valueTypes;
            }
            return "";
        }
  


        protected XmlNode _innerxml;
        private XmlDocument _doc = new XmlDocument();
        protected string _filename;

        public Schema(XmlNode schema)
        {
            _innerxml = schema;
        }

        public Schema(string filename)
        {
            try
            {
                using (FileStream fileReader = new FileStream(filename, FileMode.Open))
                using (XmlReader reader = XmlReader.Create(fileReader))
                {
                    _doc.Load(reader);
                    _innerxml = _doc;
                    _filename = filename;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message + ex.ToString() +
                                                    Environment.NewLine);
                throw;
            }
        }


        /// <summary>
        /// Returns the relevant xmlSchema node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public XmlNode GetObjectSchema(EngineObject engineObject)
        {
            //try this

            string xPath = "//" + engineObject.FullyQualifiedName.Replace('.', '/') + "/Type[@constraint='" + engineObject.ObjectType + "']/..";
            XmlNodeList candidates = _innerxml.SelectNodes(xPath);
            if (candidates.Count==0)
            {
                xPath = "//"+engineObject.FullyQualifiedName.Replace('.', '/');
                candidates = _innerxml.SelectNodes(xPath);
            }
            //Now evaluate the depends and pick the correct model
            foreach (XmlNode candidate in candidates)
            {
                if (candidate.Attributes[Schema.depends] != null)
                {
                    string[] depends = candidate.Attributes[Schema.depends].Value.Split('=');
                    string xpath = depends[0].Replace('[', '/');
                    string comparison = depends[1].Replace("'", "").Replace("]", string.Empty);
                    if (engineObject.SelectSingleNode(xpath).InnerText == comparison)
                    {
                        return candidate;
                    }
                }
                else
                {
                    return candidate;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns the relevant Scheme node based on the current node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public XmlNode GetObjectSchema(XmlNode node)
        {
            return _innerxml.SelectSingleNode(node.Name);
        }


        public static List<ParameterDetails> GetParametersFromXml(XmlNode node,Classifier objectTypes)
        {

            List<ParameterDetails> templist = new List<ParameterDetails>();
            try
            {
                foreach (XmlNode param in node.ChildNodes)
                {
                    if (param.Name!="#comment" && (string)Getvalue(param,Value.Type) != Schema.container_name)
                    {
                        int minOccurs = (int)Getvalue(node, Value.minOccurs);
                        int maxOccurs = (int)Getvalue(node, Value.maxOccurs);
                        if (objectTypes == Classifier.Optional && maxOccurs >= 1 && !(minOccurs == 1 && maxOccurs == 1))
                        {
                            templist.Add(ParameterDetails.LoadFromXml(param));
                        }
                        else if (objectTypes == Classifier.Required && minOccurs >= 1 && maxOccurs >= 1)
                        {
                            templist.Add(ParameterDetails.LoadFromXml(param));
                        }
                        else
                        {
                            templist.Add(ParameterDetails.LoadFromXml(param));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error reading Scheme : " + e.Message + "\n" + node.InnerXml);
            }
            return templist;
        }

        public static List<ObjectDetails> GetObjectsFromXml(XmlNode node, Classifier objectTypes)
        {
            try
            {
                List<ObjectDetails> templist = new List<ObjectDetails>();
                foreach (XmlNode param in node.ChildNodes)
                {
                    if (param.Name!="#comment" && Getvalue(param, Value.Type).ToString() == Schema.container_name)
                    {
                        int minOccurs = (int)Getvalue(node, Value.minOccurs);
                        int maxOccurs = (int)Getvalue(node, Value.maxOccurs);
                        if (objectTypes == Classifier.Optional && maxOccurs >= 1 && !(minOccurs == 1 && maxOccurs == 1))
                        {
                            templist.Add(ObjectDetails.LoadFromXml(param));
                        }
                        else if (objectTypes == Classifier.Required && minOccurs >= 1 && maxOccurs >= 1)
                        {
                            templist.Add(ObjectDetails.LoadFromXml(param));
                        }
                        else
                        {
                            templist.Add(ObjectDetails.LoadFromXml(param));
                        }
                    }
                }
                return templist;
            }
            catch(Exception e)
            {
                throw new Exception("Error reading Scheme : " + e.Message + "\n" + node.InnerXml);
            }            
        }

        public static XmlNode RemoveOptional(XmlNode inputNode)
        {
            XmlNode cNode = inputNode.Clone();
            foreach(XmlNode node in cNode.ChildNodes)
            {
                if ((int)Getvalue(node,Value.minOccurs)!=1)
                {
                    cNode.RemoveChild(node);
                }
            }
            return cNode;
        }

        public static List<string> GetValueTypesfromXml(XmlNode node)
        {
            List<string> templist = new List<string>();
            var tempval = (string)Getvalue(node, Value.valueTypes);                
            if (tempval == null) return templist;
            foreach (string val in tempval.Split(','))
            {
                templist.Add(val.Trim());
            }
            return templist;
        }

        public Dictionary<String,List<String>> Schema_Lists()
        {
            var temp = new Dictionary<string, List<string>>();
            XmlNode ConsList = _innerxml.SelectSingleNode("//ConstraintLists");
            foreach(XmlNode node in ConsList.ChildNodes)
            {
                var t = new List<string>();
                temp.Add(node.Name, node.InnerText.Trim(')').Trim('(').Split(',').ToArray().ToList<string>());
            }
            return temp;
        }

        public static object Getvalue(XmlNode node, Value ValToget)
        {
            switch (ValToget)
            {
                case Value.Name: return TryGetValue(ValueToString(ValToget), node, () => { return node.Name;});
                case Value.Type: return TryGetValue(ValueToString(ValToget), node, () => { return (node.Attributes[Schema.type]==null)? "container" : node.Attributes[Schema.type].Value; });
                case Value.Description: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.desc]?.Value; });
                case Value.UniqueScopeTo: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.UniqueScopeTo]?.Value; });
                case Value.bounds: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.bounds]?.Value; });
                case Value.constraint: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.constraint]?.Value; });
                case Value.defaultType: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.defaultType]?.Value; });
                case Value.defaultval: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.defaultValue]?.Value; });
                case Value.depends: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.depends]?.Value; });
                case Value.isUniqueScope: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.isUniqueScope]?.Value == "true"; });
                case Value.minOccurs: return TryGetValue(ValueToString(ValToget), node, () => { return int.Parse((node.Attributes[Schema.minOccurs]?.Value == "" || node.Attributes[Schema.minOccurs] == null) ? "0" : node.Attributes[Schema.minOccurs]?.Value); });
                case Value.maxOccurs: return TryGetValue(ValueToString(ValToget), node, () => { return int.Parse((node.Attributes[Schema.maxOccurs]?.Value == "" || node.Attributes[Schema.maxOccurs] == null) ? "999999" : node.Attributes[Schema.maxOccurs]?.Value); });
                case Value.metaType: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.metaType]?.Value; });
                case Value.reference: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.reference]?.Value; });
                case Value.referenceField: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.referenceField]?.Value; });
                case Value.referenceMetaType: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.referenceMetaType]?.Value; });
                case Value.referenceSubType: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.referenceMetaType]?.Value; });
                case Value.subType: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.subType]?.Value; });
                case Value.unique: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.unique]?.Value; });
                case Value.uniqueSearchRef: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.uniqueSearchRef]?.Value; });
                case Value.valueTypes: return TryGetValue(ValueToString(ValToget), node, () => { return node.Attributes[Schema.valueTypes]?.Value; });

                default:
                    {
                        throw new Exception("Valuetype : " + ValToget + " is not valid");
                    }

            }

        }

        private delegate object GetVal();

        private static string GetFullyQualifiedName(XmlNode node)
        {
            string parent = "";
            XmlNode currentNode = node;
            while (currentNode.ParentNode != null)
            {
                if (currentNode.ParentNode.Name != "#document")
                {
                    var pname = currentNode.ParentNode.Name;
                    if (!pname.Any(x => char.IsUpper(x)))
                    {
                        pname = char.ToUpper(pname[0]) + pname.Substring(1);
                    }
                    parent = pname + "." + parent;
                }
                currentNode = currentNode.ParentNode;
            }
            return parent + node.Name;
        }

        private static object TryGetValue(string Value, XmlNode node, GetVal getvalueFunction )
        {
            try
            {
                var val =  getvalueFunction();
                return val;
            }
            catch (Exception e)
            {
                throw new Exception( Value+ " parameter is not correctly populated in : \n"+ GetFullyQualifiedName(node) + "\n" + e.Message + "\n" + node.OuterXml);
            }
        }





    }
}
