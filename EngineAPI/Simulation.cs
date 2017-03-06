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
using System.Diagnostics;

namespace EngineAPI
{
    public class Simulation :EngineObject
    {

        private readonly XmlDocument _xmlDoc = new XmlDocument();
        private string _filename;

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
                if (filename != "")
                {
                    using (FileStream fileReader = new FileStream(filename, FileMode.Open))
                    using (XmlReader reader = XmlReader.Create(fileReader))
                    {
                        _xmlDoc.Load(reader);
                    }
                }
                else
                {
                    XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = _xmlDoc.DocumentElement;
                    _xmlDoc.InsertBefore(xmlDeclaration, root);

                    XmlElement simnode = _xmlDoc.CreateElement(string.Empty, "Simulation", string.Empty);
                    _xmlDoc.AppendChild(simnode);
                    
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

        public void AddScenarioFile(string ModelID, string outputFile, string ValueType="", string Maturities="")
        {
            var ScenarioFiles = FindObjectbyNodeName("ScenarioFiles");
            if (ScenarioFiles==null)
            {
                ScenarioFiles=this.FindObjectbyNodeName("OutputRequirements").AddObject("ScenarioFiles");
            }
            var ScenarioFile = ScenarioFiles.AddObject("ScenarioFile");
            
            ScenarioFile.Parameters["ModelID"].Value = ModelID;
            ScenarioFile.Parameters["FileName"].Value = outputFile;
            var valueType_model = ScenarioFile.FindObjectbyNodeName("ValueTypes");
            valueType_model.Parameters["ValueType"].Value = ValueType;
            if (Maturities!="")
            {
                var Zeroprice = valueType_model.AddObject("ZeroPrice");
                Zeroprice.Parameters["Maturities"].Value = Maturities;
            }
        }

        public void AddAllScenarioFiles(string outputfile, string Maturities="")
        {
            var modelList = this.FindObjectsbyNodeName("Model");
            var filename = Path.GetDirectoryName(outputfile) +"\\"+ Path.GetFileNameWithoutExtension(outputfile);
            var ext = Path.GetExtension(outputfile);

            foreach (EngineObject model in modelList)
            {
                string ModelID = model.Parameters["ModelID"].Value.ToString();
                AddScenarioFile(ModelID,filename+"_"+ModelID+ext, "", Maturities);
            }
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

        public void Run()
        {

            this.Run(this.FindObjectbyNodeName("Params").Parameters["OutputFile"].Value.ToString());            
        }


        public void Run(string outputfile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(outputfile)))
            {
                outputfile = Path.GetDirectoryName(_filename) + @"\" + Path.GetFileName(outputfile);
            }

            string outpath = System.IO.Path.GetTempPath() + "\\scendata.xml";

            this.SilentSaveAs(outpath);

            string UnitTestHarness = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\UnitTestHarness.exe";

            if (File.Exists(UnitTestHarness))
            {
                ProcessStartInfo start = new ProcessStartInfo();
                // Enter in the command line arguments, everything you would enter after the executable name itself
                start.Arguments = @"--forceoutput --testdata """ + outpath + @" "" --compdata c:\res.csv --csvresdata """ + outputfile+ "\"";

                // Enter the executable to run, including the complete path
                start.FileName = UnitTestHarness;
                // Do you want to show a console window?
                start.WindowStyle = ProcessWindowStyle.Normal;
                start.CreateNoWindow = true;
                int exitCode;
                // Run the external process & wait for it to finish
                using (Process proc = Process.Start(start))
                {
                    proc.WaitForExit();

                    // Retrieve the app's exit code
                    exitCode = proc.ExitCode;
                }
            }
            else
            {
                throw new Exception("The engine can not be found.");
            }
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
        }

        public void RemoveOutputs()
        {
            _innerXml.SelectSingleNode("//Queries").RemoveAll();
            _innerXml.SelectSingleNode("//Operators").RemoveAll();
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
            _filename = filename;
        }

        public void New()
        {
            //Search through the schema from the simulation node and add all object/parameters that have a minOccurs of 1
            _xmlDoc.RemoveAll();
            _xmlDoc.AppendChild(_schema.CreateEmptySim(_xmlDoc));
        }


        public void UpdateSimulation()
        {
            throw new NotImplementedException();
        }

        public void UpdateCalibration(string CalibrationFile)
        {
            XmlDocument DetCalibration = new XmlDocument();

            DetCalibration.Load(System.IO.Path.GetDirectoryName(CalibrationFile));

            var ESG = this.FindObjectbyNodeName("EconomicScenarioGenerator");
            ESG.RemoveAll();

            XmlNode Calibration = DetCalibration.SelectSingleNode("//EconomicScenarioGenerator");

            foreach (XmlNode node in Calibration.ChildNodes)
            {
                ESG.AddXml(node);
            }
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        public void SilentSaveAs(string filename)
        {
            _xmlDoc.Save(filename);
        }


    }
}
