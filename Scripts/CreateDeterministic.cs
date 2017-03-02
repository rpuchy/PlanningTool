using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineAPI;
using System.Xml;

namespace Scripts
{
    public class TestingData
    {

        public static void CreateTestingData(string in_filename, string out_filename)
        {
            Simulation sim = new Simulation(in_filename, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\EngineInputTemplateTypeDefines.xml");

            sim.RemoveOutputs();

            var Models = sim.FindObjectsbyNodeName("Model");
            var Products = sim.FindObjectbyNodeName("TaxWrappers").FindObjectsbyNodeName("Product");
            

            foreach (var Model in Models)
            {
                Model.AddOutputs(Model.AddableValueTypes(), "SCENARIOALL");
            }
            foreach (var Product in Products)
            {
                Product.AddOutputs(Product.AddableValueTypes(), "SCENARIOALL");
            }

            //Now Switch out the Calibration

            XmlDocument DetCalibration = new XmlDocument();

            DetCalibration.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\DeterministicCalibration.xml");

            var Models_node = sim.FindObjectbyNodeName("Models");
            Models_node.RemoveAll();

            XmlNode Calibration = DetCalibration.SelectSingleNode("//Models");

            foreach(XmlNode node in Calibration.ChildNodes)
            {
                Models_node.AddXml(node);
            }

            sim.SaveAs(out_filename);

        }

    }
}
