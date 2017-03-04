using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using EngineAPI;
using System.Xml;
using System.Text.RegularExpressions;

namespace Scripts
{

    public class CalibData : DependencyObject
    {
        public static readonly DependencyProperty CalibrationFileProperty =
          DependencyProperty.Register("CalibrationFile", typeof(string),
          typeof(CalibData), new UIPropertyMetadata(null));

        public string CalibrationFile
        {
            get { return (string)GetValue(CalibrationFileProperty); }
            set { SetValue(CalibrationFileProperty, value); }
        }

        public static readonly DependencyProperty CurrencyProperty =
          DependencyProperty.Register("Currency", typeof(string),
          typeof(CalibData), new UIPropertyMetadata(null));

        public string Currency
        {
            get { return (string)GetValue(CurrencyProperty); }
            set { SetValue(CurrencyProperty, value); }
        }

        public static readonly DependencyProperty AssumptionSetProperty =
            DependencyProperty.Register("AssumptionSet", typeof(string),
            typeof(CalibData), new UIPropertyMetadata(null));

        public string AssumptionSet
        {
            get { return (string)GetValue(AssumptionSetProperty); }
            set { SetValue(AssumptionSetProperty, value); }
        }
    }

    public class CalibrationTemplate
    {

        private static string FormatName(string oldName)
        {
            if (oldName.Any(c => char.IsUpper(c)))
            {
                //nothing to be done here
                return oldName;
            }
            else
            {
                return EngineObject.GetAlternate(oldName); 
            }
        }

        private static XmlElement processModel(XmlDocument doc, EngineObject node)
        {
            XmlElement element = doc.CreateElement(string.Empty, node.Name, string.Empty);
            foreach (var param in node.Parameters)
            {
                var newparam = doc.CreateElement(string.Empty, FormatName(param.Name), string.Empty);
                newparam.AppendChild(doc.CreateTextNode(param.Value.ToString()));
                element.AppendChild(newparam);
            }
            foreach (var child in node.Children)
            {
                var newelement = processModel(doc, child);
                element.AppendChild(newelement);
            }
            return element;
        }

        public static void CreateTemplate(string outputfilename, List<CalibData> calibrationData)
        {

            //Load file that dictates model order
            Dictionary<string, int> ModelOrder = new Dictionary<string, int>();
            using (var fs = File.OpenRead(@".\ModelOrder.csv"))
            using (var reader = new StreamReader(fs))
            {
                reader.ReadLine(); //read over header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    //remove whitespace from items.
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim();
                    }
                    ModelOrder.Add(values[1], int.Parse(values[0]));
                }
            }


            List<EngineObject> calibObjects = new List<EngineObject>();

            foreach (CalibData data in calibrationData)
            {
                calibObjects.Add(new Simulation(data.CalibrationFile));
            }

            DateTime EffectiveDate = DateTime.Parse(calibObjects[0].FindObjectbyNodeName("Params").Parameters["EffectiveDate"].ToString());

            XmlDocument doc = new XmlDocument();

            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement xmlCalibrations = doc.CreateElement(string.Empty, "Calibrations", string.Empty);
            doc.AppendChild(xmlCalibrations);

            XmlElement xmlCalibrationTemplates = doc.CreateElement(string.Empty, "CalibrationTemplate", string.Empty);
            xmlCalibrations.AppendChild(xmlCalibrationTemplates);

            XmlElement xmlEffectiveDate = doc.CreateElement(string.Empty, "EffectiveDate", string.Empty);
            xmlCalibrationTemplates.AppendChild(xmlEffectiveDate);
            xmlEffectiveDate.AppendChild(doc.CreateTextNode(EffectiveDate.ToString("O").Substring(0, EffectiveDate.ToString("O").Length - 4)));

            XmlElement xmlTemplateGroup = doc.CreateElement(string.Empty, "TemplateGroup", string.Empty);
            xmlCalibrationTemplates.AppendChild(xmlTemplateGroup);
            xmlTemplateGroup.AppendChild(doc.CreateTextNode("AMP"));



            for (int i = 0; i < calibObjects.Count; i++)
            {

                var Models = calibObjects[i].FindObjectbyNodeName("Models");

                XmlElement xmleconomicmodels = doc.CreateElement(string.Empty, "economic_models", string.Empty);
                xmlCalibrationTemplates.AppendChild(xmleconomicmodels);

                XmlElement xmlassumptionSet = doc.CreateElement(string.Empty, "assumption_set", string.Empty);
                xmleconomicmodels.AppendChild(xmlassumptionSet);
                xmlassumptionSet.AppendChild(doc.CreateTextNode(calibrationData[i].AssumptionSet));

                XmlElement xmlEffectiveDate2 = doc.CreateElement(string.Empty, "effective_date", string.Empty);
                xmleconomicmodels.AppendChild(xmlEffectiveDate2);
                xmlEffectiveDate2.AppendChild(doc.CreateTextNode(EffectiveDate.ToString("dd/MM/yyyy")));

                //This is where we cycle models.

                Dictionary<int, XmlNode> modelXMLSnippet = new Dictionary<int, XmlNode>();

                foreach (var model in Models.Children)
                {

                    string type = model.Parameters["Type"]?.ToString();

                    if (type == null)
                    {
                        type = model.Parameters["Class"].ToString();
                    }

                    if (type == "EQUITY" || type == "CHILDEQUITY" || type == "TWOFACTORHULLWHITE" ||
                        type == "DETERMINISTICEQUITY" || type == "ORNSTEINUHLENBECK")
                    {
                        //we only add the main model as the link then add the dependencies
                        XmlElement xmleconomicmodel = doc.CreateElement(string.Empty, "economic_model", string.Empty);
                        //xmleconomicmodels.AppendChild(xmleconomicmodel);

                        string modelname = "";
                        string calibrationname = "";
                        if (type == "TWOFACTORHULLWHITE")
                        {
                            modelname = "AustralianYieldCurve";
                            calibrationname = modelname.ToUpper() + "-2FHW";
                        }
                        else
                        {
                            modelname = model.Parameters["Name"].ToString().Replace(" ", string.Empty);
                            calibrationname = modelname;
                        }

                        //Now are add the type of the model
                        if (type == "DETERMINISTICEQUITY")
                        {
                            calibrationname = modelname + "-DETERMINISTIC";
                        }

                        if ((type == "TWOFACTORHULLWHITE") &&
                            (Double.Parse(
                                 model.FindObjectbyNodeName("ModelParameters").Parameters["VolatilityShortRate"].ToString()) <
                             0.000001)
                            &&
                            (Double.Parse(
                                 model.FindObjectbyNodeName("ModelParameters").Parameters["VolatilityAdditionalParam"].ToString()) <
                             0.000001))
                        {
                            calibrationname = modelname + "-DETERMINISTIC";
                        }

                        if (type == "EQUITY")
                        {
                            //identify the type of the vol model
                            string voltype =
                                Models.FindObjectbyParamValue(model.Parameters["VolatilityModelID"].ToString(), "ModelID")
                                    .Parameters[
                                        "Type"].ToString();

                            calibrationname = modelname + "-" + voltype;
                        }

                        if (type == "ORNSTEINUHLENBECK")
                        {
                            calibrationname = modelname + "-" + type;
                            if (Double.Parse(model.Parameters["Sigma"].ToString()) < 0.00001)
                            {
                                calibrationname = calibrationname + "-DETERMINISTIC";
                            }
                        }

                        XmlElement xmlModelName = doc.CreateElement(string.Empty, "model_name", string.Empty);
                        xmleconomicmodel.AppendChild(xmlModelName);
                        xmlModelName.AppendChild(doc.CreateTextNode(modelname));

                        XmlElement xmlCurrency = doc.CreateElement(string.Empty, "currency", string.Empty);
                        xmleconomicmodel.AppendChild(xmlCurrency);
                        xmlCurrency.AppendChild(doc.CreateTextNode(calibrationData[i].Currency));

                        XmlElement xmlcurrentmeanreturn = doc.CreateElement(string.Empty, "current_mean_return",
                            string.Empty);
                        xmleconomicmodel.AppendChild(xmlcurrentmeanreturn);
                        xmlcurrentmeanreturn.AppendChild(doc.CreateTextNode("0"));

                        XmlElement xmltypes = doc.CreateElement(string.Empty, "types", string.Empty);
                        xmleconomicmodel.AppendChild(xmltypes);

                        XmlElement xmlcalibrationtype = doc.CreateElement(string.Empty, "calibration_type", string.Empty);
                        xmltypes.AppendChild(xmlcalibrationtype);

                        xmlcalibrationtype.AppendChild(doc.CreateTextNode(calibrationname));

                        XmlElement xmlmodels = doc.CreateElement(string.Empty, "models", string.Empty);
                        xmleconomicmodel.AppendChild(xmlmodels);

                        //Add the model, it's volatility model and it's income model if they exists
                        EngineObject volmodel = null;
                        if (model.Parameters["VolatilityModelID"] != null &&
                            model.Parameters["VolatilityModelID"].ToString() != "0")
                        {
                            volmodel = Models.FindObjectbyParamValue(model.Parameters["VolatilityModelID"].ToString(), "ModelID");
                        }

                        EngineObject incomemodel = null;
                        if (model.Parameters["IncomeModelID"] != null &&
                            model.Parameters["IncomeModelID"].ToString() != "0")
                        {
                            incomemodel = Models.FindObjectbyParamValue(model.Parameters["IncomeModelID"].ToString(), "ModelID");
                        }

                        xmlmodels.AppendChild(processModel(doc, model));
                        if (volmodel != null)
                        {
                            xmlmodels.AppendChild(processModel(doc, volmodel));
                        }
                        if (incomemodel != null)
                        {
                            xmlmodels.AppendChild(processModel(doc, incomemodel));
                        }
                        int res;
                        if (ModelOrder.TryGetValue(model.Parameters["Name"].ToString().Replace(" ", string.Empty),
                            out res))
                        {
                            modelXMLSnippet.Add(res, xmleconomicmodel);
                        }
                        else
                        {
                            xmleconomicmodels.AppendChild(xmleconomicmodel);

                        }
                    }

                }
                for (int j = 1; j <= modelXMLSnippet.Count; j++)
                {
                    xmleconomicmodels.AppendChild(modelXMLSnippet[j]);
                }
                //Now we add the correlations
                EngineObject correlations = calibObjects[i].FindObjectbyNodeName("Correlations");
                XmlElement xmlCorrelations = doc.CreateElement(string.Empty, "Correlations", string.Empty);
                xmlCalibrationTemplates.AppendChild(xmlCorrelations);

                XmlElement xmlAssumptionsSet2 = doc.CreateElement(string.Empty, "assumption_set", string.Empty);
                xmlCorrelations.AppendChild(xmlAssumptionsSet2);

                xmlAssumptionsSet2.AppendChild(doc.CreateTextNode(calibrationData[i].AssumptionSet));

                XmlElement xmlEffectivedate3 = doc.CreateElement(string.Empty, "effective_date", string.Empty);
                xmlCorrelations.AppendChild(xmlEffectivedate3);

                xmlEffectivedate3.AppendChild(doc.CreateTextNode(EffectiveDate.ToString("dd/MM/yyyy")));

                foreach (EngineObject child in correlations.Children)
                {
                    xmlCorrelations.AppendChild(processModel(doc, child));
                }
            }

            doc.Save(outputfilename);
        }


    }
}
