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
        
        //public class MenuItem
        //{
        //    public MenuItem()
        //    {
        //        this.Items = new ObservableCollection<MenuItem>();
        //    }

        //    public string Title { get; set; }

        //    public ObservableCollection<MenuItem> Items { get; set; }
        //}

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

        private void upDateParameters(EngineObjectViewModel _obj)
        {
            _parameters.Clear();

            foreach (var param in _obj.Parameters)
            {
                var p = new Parameter() { Name = param.Name, Value = param.Value };
                _parameters.Add(p);
            }
        }

        void InterfaceTreeViewComputers_SelectionChange(Object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            upDateParameters((EngineObjectViewModel)e.NewValue);
            
            AddressBox.Text = ((EngineObjectViewModel) e.NewValue).Fullyqualifiedname;
            Selecteditem = (EngineObjectViewModel)e.NewValue;

            if (TreeviewControl.EngineObjectViewTree.ContextMenu != null)
            {
                TreeviewControl.EngineObjectViewTree.ContextMenu.Items.Clear();
            }

            ContextMenu context = new ContextMenu();
            MenuItem Remove = new MenuItem();
            Remove.Header = "Remove";
            Remove.Click += delegate { Selecteditem.RemoveObject(); TreeviewControl.EngineObjectViewTree.Items.Refresh(); };
            context.Items.Add(Remove);
            MenuItem Add = new MenuItem();
            Add.Header = "Add";
            foreach(string _addobj in Selecteditem.AddableObjects)
            {
                var mn = new MenuItem();
                mn.Header = _addobj;
                mn.Click += delegate { Selecteditem.AddObject(_addobj.Split('-')[0],(_addobj.Split('-').Count()>1)? _addobj.Split('-')[1]:""); TreeviewControl.EngineObjectViewTree.Items.Refresh(); };
                Add.Items.Add(mn);
            }
            context.Items.Add(Add);
            TreeviewControl.EngineObjectViewTree.ContextMenu = context;


            if (listView.ContextMenu != null)
            {
                listView.ContextMenu.Items.Clear();
            }

            ContextMenu LV_context = new ContextMenu();
            MenuItem LV_Remove = new MenuItem();
            LV_Remove.Header = "Remove";
            LV_Remove.Click += delegate { MessageBox.Show("Remove"); };
            LV_context.Items.Add(LV_Remove);
            MenuItem LV_Add = new MenuItem();
            LV_Add.Header = "Add";
            foreach (string _addobj in Selecteditem.AddableParameters)
            {
                var mn = new MenuItem();
                mn.Header = _addobj;
                mn.Click += delegate { Selecteditem.AddParameter(_addobj); upDateParameters(Selecteditem); };
                LV_Add.Items.Add(mn);
            }
            LV_context.Items.Add(LV_Add);
            listView.ContextMenu = LV_context;







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
            if (Sim != null)
            {
                Sim.Save();
            }
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

            }
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            if (Sim != null)
            {
                Sim.RemoveOutputs();
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
            Sim.Run();            
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
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
                    Deterministic.CreateDeterministicSim(tempRequest, filename);
                }

            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {

        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var param = (TextBox)sender;
            Selecteditem.Parameters[_parameters[listView.SelectedIndex].Name].Value = param.Text;
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
