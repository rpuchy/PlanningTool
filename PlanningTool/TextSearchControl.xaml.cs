using System;
using System.Diagnostics.PerformanceData;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EngineAPI;

namespace PlanningTool
{
    public partial class TextSearchControl : UserControl
    {
        private EngineObjectViewModel _engineObjectTree; 

        public EngineObjectViewModel EngineObjectTree
        {
            get { return _engineObjectTree; }
            set { _engineObjectTree = value; }
        }

        public TextSearchControl()
        {
            InitializeComponent();
            
            //// Get raw family tree data from a database.
            //Simulation rootObject = EngineObjectTree;

            //// Create UI-friendly wrappers around the 
            //// raw data objects (i.e. the view-model).
            //_tree = new TreeViewModel(rootObject);

            //// Let the UI bind to the view-model.
            //base.DataContext = _tree;            
        }

        public void SetData(TreeViewModel data)
        {
            base.DataContext = data;
        }

        void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ((TreeViewModel)base.DataContext).SearchCommand.Execute(null);
        }
    }
}