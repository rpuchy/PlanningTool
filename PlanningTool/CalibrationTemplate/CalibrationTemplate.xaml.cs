using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace PlanningTool
{
    /// <summary>
    /// Interaction logic for CalibrationTemplate.xaml
    /// </summary>
    public partial class CalibrationTemplate : Window
    {

        private ObservableCollection<CalibData> _CalibFileCollection =
      new ObservableCollection<CalibData>();

        private ObservableCollection<string> _AvailableCurrencies =  new ObservableCollection<string>();
        private ObservableCollection<string> _AvailableAssumptionSets = new ObservableCollection<string>();

        public CalibrationTemplate()
        {
            InitializeComponent();

            _AvailableCurrencies.Add("AUD");

            _AvailableAssumptionSets.Add("MillimanAustraliaDefault");
            _AvailableAssumptionSets.Add("MillimanAustraliaDeterministic");

            _CalibFileCollection.Add(new CalibData() {AssumptionSet = "MillimanAustraliaDefault", Currency = "AUD"});
            _CalibFileCollection.Add(new CalibData() { AssumptionSet = "MillimanAustraliaDeterministic", Currency = "AUD"});


        }

        private void AddRowButtonBase_OnClick_(object sender, RoutedEventArgs e)
        {
            _CalibFileCollection.Add(new CalibData() {CalibrationFile = "",Currency = "AUD", AssumptionSet = "MillimanAustraliaDefault" });
        }

        public ObservableCollection<CalibData> CalibFileCollection
        { get { return _CalibFileCollection;} }

        public ObservableCollection<string> AvailableCurrencies
        { get { return _AvailableCurrencies; } }

        public ObservableCollection<string> AvailableAssumptionSets
        { get { return _AvailableAssumptionSets; } }

        private void BrowseForCalibFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                string fileName = openFileDialog1.FileName;
                _CalibFileCollection[calibFileListView.SelectedIndex].CalibrationFile = fileName;
            }
        }

        private void CreateTemplate(object sender, RoutedEventArgs e)
        {
            //First choose a file.
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Calibrations"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

               // FileOpsImplementation.CalibrationsTemplate(filename, CalibFileCollection.ToList());

            }
        }
    }




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
}
