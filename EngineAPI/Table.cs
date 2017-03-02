using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    public class Table
    {

        private XmlNode _innerXml;

        public Table(XmlNode node)
        {
            _innerXml = node;
        }


        public List<string> Columns
        {
            get
            {
                var temp = new List<string>();
                foreach (XmlNode child in _innerXml.FirstChild.ChildNodes)
                {
                    temp.Add(child.Name);
                }
                return temp;
            }
        }

        public List<List<String>> Data
        {
            get
            {
                List<List<String>> temp = new List<List<String>>();
                foreach(XmlNode child in _innerXml.ChildNodes)
                {
                    var tdata_temp = new List<string>();
                    foreach(XmlNode tData in child.ChildNodes)
                    {
                        tdata_temp.Add(tData.InnerText);
                    }
                    temp.Add(tdata_temp);
                }
                return temp;
            }
        }

    }
}
