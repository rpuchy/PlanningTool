using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using EngineAPI;

namespace PlanningTool
{
    /// <summary>
    /// A UI-friendly wrapper around an Engine object.
    /// </summary>
    public class EngineObjectViewModel :  INotifyPropertyChanged
    {
        #region Data

        readonly EngineObject _engineObject;
        readonly EngineObjectViewModel _parent;

        private string _nodeName;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public EngineObjectViewModel(EngineObject engineObject, EngineObjectViewModel parent)         
        {
            _engineObject = engineObject;
            _parent = parent;
        }

        //private EngineObjectViewModel(EngineObject engineObject)
        //{
        //    _engineObject = engineObject;
        //    _parameters = engineObject.Parameters;
        //    if (parent == null)
        //    {
        //        _fullyqualifiedname = _engineObject.Name;
        //    }
        //    else
        //    {
        //        _fullyqualifiedname = parent.Fullyqualifiedname + '.' + _engineObject.Name;
        //    }
            
        //    _children = new ReadOnlyCollection<EngineObjectViewModel>(
        //            (from child in _engineObject.Children
        //             select new EngineObjectViewModel(child, this))
        //             .ToList<EngineObjectViewModel>());
        //}

        #endregion // Constructors

        #region EngineObject Properties

        public ReadOnlyCollection<EngineObjectViewModel> Children
        {
            get
            {
               return  new ReadOnlyCollection<EngineObjectViewModel>(
                    (from child in _engineObject.Children
                     select new EngineObjectViewModel(child, this))
                     .ToList<EngineObjectViewModel>());                
            }
        }

        public string Name
        {
            get { return _engineObject.Name; }
        }

        public string Fullyqualifiedname
        {
            get { return _engineObject.FullyQualifiedName; }
        }

        public ParamList Parameters
        {
            get { return _engineObject.Parameters; }
        }

        public string NodeName
        {
            get { return _engineObject.Name; }
            
        }

        #endregion // EngineObject Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public EngineObjectViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}