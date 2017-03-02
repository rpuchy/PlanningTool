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
            var Products = sim.FindObjectsbyNodeName("Product");

            foreach (var Model in Models)
            {
                Model.AddOutput(Model.AddableValueTypes()[0], "SCENARIOALL");
            }
            foreach (var Product in Products)
            {
                Product.AddOutputs(Product.AddableValueTypes(), "SCENARIOALL");
            }

            sim.SaveAs(out_filename);

        }

    }
}
