using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineAPI;
using MillimanMath;
using System.IO;

namespace Scripts
{
    public static class Deterministic
    {

        public static void CreateDeterministicSim(string in_filename, string out_filename)
        {
            Simulation sim = new Simulation(in_filename, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\EngineInputTemplateTypeDefines.xml");

            sim.RemoveOutputs();

            var hw = sim.FindObjectbyName("AUYieldCurve").FindObjectbyNodeName("ModelParameters");
            hw.Parameters["VolatilityShortRate"].Value = "0.000000000001";
            hw.Parameters["VolatilityAdditionalParam"].Value = "0.000000000001";


            var cpi = sim.FindObjectbyName("CPI");
            cpi.Parameters["Sigma"].Value = "0.0000000000001";
            //TODO: we should set the mpr to mpr*sigmaold/sigmanew
            var awe = sim.FindObjectbyName("AWE");
            awe.Parameters["Sigma"].Value = "0.0000000000001";

            //Create one const vol model and set all other assumptions to correct mean return in model.
            //Find all models of type equity
            var modeList = sim.FindObjectsbyNodeName("Model");
            var Models = sim.FindObjectbyNodeName("Models");

            var medians = CreateMedianData(in_filename);

            foreach (EngineObject model in modeList)
            {
                if (model.ObjectType == "EQUITY" || model.ObjectType == "CHILDEQUITY")
                {
                    var Detmodel = Models.AddObject("Model", "DETERMINISTICEQUITY");

                    Detmodel.Parameters["ModelID"].Value = model.Parameters["ModelID"].Value;
                    Detmodel.Parameters["Name"].Value = model.Parameters["Name"].Value;
                    Detmodel.Parameters["UseNominalRates"].Value = model.Parameters["UseNominalRates"].Value;
                    Detmodel.Parameters["NominalRatesModelID"].Value = model.Parameters["NominalRatesModelID"].Value;
                    Detmodel.Parameters["MeanReturn"].Value = Math.Round(medians[model.Parameters["ModelID"].Value.ToString()], 8);
                    Detmodel.Parameters["IncomeModelID"].Value = model.Parameters["IncomeModelID"].Value;
                }
            }

            Models.RemoveAll("EQUITY");
            Models.RemoveAll("CHILDEQUITY");
            Models.RemoveAll("REGIMESWITCHVOLATILITY");
            Models.RemoveAll("CONSTANTVOLATILITY");

            sim.FindObjectbyNodeName("Correlations").RemoveAll();
            sim.SaveAs(out_filename);
        }


        private static Dictionary<string, double> CreateMedianData(string simfile)
        {
            //this method will take in a sim file
            //Add scenario file outputs then process the outputs

            Simulation sim = new Simulation(simfile, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\EngineInputTemplateTypeDefines.xml");


            var Models = sim.FindObjectsbyNodeName("Model");

            foreach(var Model in Models)
            {
                if (Model.ObjectType=="EQUITY"|| Model.ObjectType == "CHILDEQUITY")
                {
                    Model.Parameters["UseNominalRates"].Value = "false";
                }
            }

            var Params = sim.FindObjectbyNodeName("Params");

            Params.Parameters["Scenarios"].Value = "30000";

            Params.Parameters["TimeSteps"].Value = "30";

            Params.RemoveObjects("TimeStepSizes");


            sim.AddAllScenarioFiles(System.IO.Path.GetTempPath() + "\\ScenarioFile.csv");

            sim.RemoveOutputs();

            sim.Run(System.IO.Path.GetTempPath() + "\\Scendata.csv");
                        
            Dictionary<string, double> tempres = new Dictionary<string, double>();

            foreach (var scenarioFile in sim.FindObjectsbyNodeName("ScenarioFile"))
            {
                List<double> ScenarioData = new List<double>();

                using (var fs = File.OpenRead(scenarioFile.Parameters["FileName"].Value.ToString()))
                using (var reader = new StreamReader(fs))
                {
                    //skip the first line
                    var header = reader.ReadLine().Split(',');
                    //Figure out the index to use
                    int headertouse = 0;
                    for (int j = 0; j < header.Length; j++)
                    {
                        if (header[j] == "ROLLUP")
                        {
                            headertouse = j;
                        }

                    }
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (int.Parse(values[1]) == 30)
                        {
                            var val = Math.Log(double.Parse(values[headertouse])) / 30;
                            ScenarioData.Add(val);
                        }
                    }                   
                }

                tempres.Add(scenarioFile.Parameters["ModelID"].Value.ToString(), Basic.CalcMedian(ScenarioData));
            }
            return tempres;

        }

    }
        
}
