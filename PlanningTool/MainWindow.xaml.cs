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
using EngineAPI;
using Scripts;

namespace PlanningTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Simulation Sim;
        private TreeViewModel VisualData;
        private EngineObjectViewModel Selecteditem;           


        ObservableCollection<Parameter> _parameters = new ObservableCollection<Parameter>();

        public MainWindow()
        {
            InitializeComponent();
            TreeviewControl.EngineObjectViewTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<Object>(InterfaceTreeViewComputers_SelectionChange);
            _parameters.CollectionChanged += _parameters_CollectionChanged;
            base.DataContext = this;
        }

        private void _parameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems!=null)
            {
                foreach (Parameter param in e.OldItems)
                {
                    //Removed items
                    param.PropertyChanged -= Param_PropertyChanged;
                }
            }
            else if (e.NewItems!=null)
            {
                foreach (Parameter param in e.NewItems)
                {
                    //Added items
                    param.PropertyChanged += Param_PropertyChanged;
                }
            }
        }

        public ObservableCollection<Parameter> Parameters
        {
            get { return _parameters; }
        }


    
        private void Window_Initialized(object sender, EventArgs e)
        {

        
        }
        
        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                string fileName = openFileDialog1.FileName;
                Sim = new Simulation(fileName, System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\EngineInputTemplateTypeDefines.xml");
            }            
            VisualData = new TreeViewModel(Sim);
            TreeviewControl.SetData(VisualData);
            
            base.DataContext = TreeviewControl;
            listView.DataContext = this;            
        }

        void InterfaceTreeViewComputers_SelectionChange(Object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _parameters.Clear();
            
            foreach (var param in ((EngineObjectViewModel)e.NewValue).Parameters)
            {
                var p = new Parameter() { Name = param.Name, Value = param.Value };
                _parameters.Add(p);
            }
            
            AddressBox.Text = ((EngineObjectViewModel) e.NewValue).Fullyqualifiedname;
            Selecteditem = (EngineObjectViewModel)e.NewValue;
        }

        private void Param_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var param = (Parameter)sender;
            Selecteditem.Parameters[param.Name].Value = param.Value;
        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            object obj = item.Content;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //fOps.UpdateModel(VisualData.FirstGeneration[0]);
            //fOps?.Save();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (Sim != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xml"; // Default file extension
                dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

                // Show save file dialog box
                bool? result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    Sim.SaveAs(filename);
                }
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (Sim != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xml"; // Default file extension
                dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

                // Show save file dialog box
                bool? result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    string tempRequest = System.IO.Path.GetTempPath() + "temp.xml";
                    Sim.SilentSaveAs(tempRequest);
                    TestingData.CreateTestingData(tempRequest, filename);
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

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            if (Sim != null)
            {
                Sim.RemoveOutputs();
                //VisualData = new TreeViewModel(fOps.EngineObjectTree);
                //TreeviewControl.SetData(VisualData);
            }
        }

        private void MenuItem_OnClick_7(object sender, RoutedEventArgs e)
        {

            new CalibrationTemplate().Show();
        }

        private void RunButton(object sender, RoutedEventArgs e)
        {
            Runsimulation();
        }

        private void Runsimulation()
        {
            // Prepare the process to run


            string UnitTestHarness = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @".\UnitTestHarness.exe";

            string tempRequest = System.IO.Path.GetTempPath() + "temp.xml";
            string tempout = System.IO.Path.GetDirectoryName(Sim.Filename) + "\\Results.csv";

            Sim.SilentSaveAs(tempRequest);

            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = @"--forceoutput --testdata """ + tempRequest + @" "" --compdata c:\res.csv --csvresdata """ + tempout + @"""";
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
    }


    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            bool param = bool.Parse(parameter as string);
            bool val = (bool)value;

            return val == param ?
              Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
