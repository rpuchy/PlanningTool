using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineAPI;
using System.Xml;
using System.IO;

namespace Scripts
{
    public class TestingData
    {

        public static void CreateTestingData(string in_filename, string out_filename)
        {
            Simulation sim = new Simulation(in_filename, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\EngineInputTemplateTypeDefines.xml");

            sim.RemoveOutputs();

            List<int> Scenario_list = new List<int>();
            Scenario_list.Add(1);

            sim.AddTransactionLog(Path.GetDirectoryName(out_filename)+"\transactionlog.csv",Scenario_list);

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

            var ESG = sim.FindObjectbyNodeName("EconomicScenarioGenerator");
            ESG.RemoveAll();

            XmlNode Calibration = DetCalibration.SelectSingleNode("//EconomicScenarioGenerator");

            foreach(XmlNode node in Calibration.ChildNodes)
            {
                ESG.AddXml(node);
            }

            sim.SaveAs(out_filename);

        }



        //string loglocation = System.IO.Path.GetDirectoryName(fOps.FileName) + "\\Transactionlog.csv";
        //fOps.AddAlloutputs( 0, 100, loglocation);
        ////Now produce rebalance rule table

        //Excel.Application xlApp = new Excel.Application();

        //Excel.Workbook xlNewWorkbook = xlApp.Workbooks.Add(Type.Missing);
        //xlApp.DisplayAlerts = false;


        //string requestanalyser = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\RequestAnalyser.xlsm";

        //Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(requestanalyser);
        //Excel.Worksheet xlWorksheet = xlWorkbook.Worksheets.get_Item("Control");

        //Excel.Range rng = xlWorksheet.get_Range("B1");
        //rng.Value = fOps.FileName;

        //xlApp.Run("ShowRebalanceRulesByTimestepPriority");

        //xlWorksheet = xlWorkbook.Worksheets.get_Item("RebalanceRules");
        //xlWorksheet.Select();
        //Excel.Range srcrange = xlWorksheet.UsedRange;
        //srcrange.Copy(Type.Missing);



        //Excel.Worksheet xlNewworksheet = (Excel.Worksheet)xlNewWorkbook.Worksheets.get_Item(1);

        //Excel.Range dstrng = xlNewworksheet.get_Range("A1");

        //dstrng.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, Type.Missing, Type.Missing);

        //xlNewWorkbook.SaveAs(System.IO.Path.GetDirectoryName(fOps.FileName) + "\\Rebalance Rule Priority.xlsx");

        //xlWorkbook.Close();
        //xlNewWorkbook.Close();
        //xlApp.Quit();

        //VisualData = new TreeViewModel(fOps.EngineObjectTree);
        //TreeviewControl.SetData(VisualData);

    }
}
