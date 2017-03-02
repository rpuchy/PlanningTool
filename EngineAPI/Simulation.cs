using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Win32;

namespace EngineAPI
{
    public class Simulation :EngineObject
    {

        private readonly XmlDocument _xmlDoc = new XmlDocument();
        private readonly string _filename;

        public string Filename { get { return _filename; } }

        public Simulation(string filename, string schemaFilename="") : base()
        {
            _filename = filename;            
            if (schemaFilename != "")
            {
                _schema = new Schema(schemaFilename);
            }
            try
            {
                using (FileStream fileReader = new FileStream(filename, FileMode.Open))
                using (XmlReader reader = XmlReader.Create(fileReader))
                {                    
                    _xmlDoc.Load(reader);                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message + ex.ToString() +
                                                    Environment.NewLine);
                throw;
            }
            _innerXml = _xmlDoc;
            SetFullyQualifiedName();
        }

        public void AddScenarioFile(string ModelID)
        {
            throw new NotImplementedException();
        }

        public void AddAllScenarioFiles()
        {
            throw new NotImplementedException();
        }

        public void SetoutputLocation(string filename)
        {
            var node = _innerXml.SelectSingleNode("//Simulation/Params/OutputFile|//Simulation/Params/output_file");
            if (node==null)
            {
                this.FindObjectbyNodeName("Params").AddParameter("OutputFile", filename);
                return;
            }
            node.InnerText = filename;
            
        }

        public void AddTransactionLog(string filename, List<int> Scenarios)
        {
            if (Scenarios.Count == 0) return;
            var Params = FindObjectbyNodeName("Params");
            var tlog_obj = Params.AddObject("TransactionLog");
            tlog_obj.Parameters["LogFile"].Value = filename;
            var ScenariosParams = tlog_obj.FindChild("Scenarios");
            if (Scenarios.Count >=1)
            {
                ScenariosParams.Parameters["Scenario"].Value = Scenarios[0];
                if (Scenarios.Count==1) return;
            }
            for (int i=1;i<Scenarios.Count;i++)
            {
                ScenariosParams.AddParameter("Scenario", Scenarios[i].ToString());
            }
            Params.removeObject(tlog_obj);
        }

        public void RemoveOutputs()
        {
            _innerXml.SelectSingleNode("\\Queries").RemoveAll();
            _innerXml.SelectSingleNode("\\Operators").RemoveAll();
        }

        public Simulation(XmlDocument doc) : base(doc,new XmlDocument())
        {
            _xmlDoc = doc;
        }

        public void Save()
        {
            if (_filename == "")
            {
                throw new Exception("There is no filename associated with this xml");
            }
            _xmlDoc.Save(_filename);
        }

        public void SaveAs(string filename)
        {
            _xmlDoc.Save(filename);
        }

    }
}
