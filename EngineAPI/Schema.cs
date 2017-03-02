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
                    if (param.Attributes[Schema.type].Value != Schema.container_name)
                    {
                        int minOccurs = int.Parse(param.Attributes[Schema.minOccurs]?.Value);
                        int maxOccurs = param.Attributes[Schema.maxOccurs]?.Value == "" ? 9999 : int.Parse(param.Attributes[Schema.maxOccurs]?.Value);
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
                    if (param.Attributes[Schema.type].Value == Schema.container_name)
                    {
                        int minOccurs = int.Parse(param.Attributes[Schema.minOccurs]?.Value);
                        int maxOccurs = param.Attributes[Schema.maxOccurs]?.Value == "" ? 9999 : int.Parse(param.Attributes[Schema.maxOccurs]?.Value);
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
                if (int.Parse(node.Attributes["minOccurs"].Value)!=1)
                {
                    cNode.RemoveChild(node);
                }
            }
            return cNode;
        }

        public static List<string> GetValueTypesfromXml(XmlNode node)
        {
            List<string> templist = new List<string>();
            var tempval = node.Attributes[Schema.valueTypes];
            if (tempval == null) return templist;
            foreach (string val in tempval.Value.Split(','))
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


    }
}
