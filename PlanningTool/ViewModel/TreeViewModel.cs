using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EngineAPI;


namespace PlanningTool
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class TreeViewModel
    {
        #region Data

        readonly ReadOnlyCollection<EngineObjectViewModel> _firstNode;
        readonly EngineObjectViewModel _rootObject;
        readonly ICommand _searchCommand;
        

        IEnumerator<EngineObjectViewModel> _matchingPeopleEnumerator;
        string _searchText = String.Empty;

        #endregion // Data

        
        #region Constructor

        public TreeViewModel(Simulation rootObject)
        {
            _rootObject = new EngineObjectViewModel((EngineObject)rootObject.FindChild("Simulation"),null);

            _firstNode = new ReadOnlyCollection<EngineObjectViewModel>(
                new EngineObjectViewModel[] 
                {
                    _rootObject
                });
            _searchCommand = new SearchFamilyTreeCommand(this);
        }

        #endregion // Constructor

        #region Properties

        #region Rootnode

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<EngineObjectViewModel> RootNode
        {
            get { return _firstNode; }
        }

        #endregion // Rootnode

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        private class SearchFamilyTreeCommand : ICommand
        {
            readonly TreeViewModel _familyTree;

            public SearchFamilyTreeCommand(TreeViewModel familyTree)
            {
                _familyTree = familyTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                _familyTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;

                _matchingPeopleEnumerator = null;
            }
        }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingPeopleEnumerator == null || !_matchingPeopleEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var person = _matchingPeopleEnumerator.Current;

            if (person == null)
                return;

            // Ensure that this person is in view.
            if (person.Parent != null)
                person.Parent.IsExpanded = true;

            person.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            var matches = this.FindMatches(_searchText, _rootObject);
            _matchingPeopleEnumerator = matches.GetEnumerator();

            if (!_matchingPeopleEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        IEnumerable<EngineObjectViewModel> FindMatches(string searchText, EngineObjectViewModel person)
        {
            if (person.NameContainsText(searchText))
                yield return person;

            foreach (EngineObjectViewModel child in person.Children)
                foreach (EngineObjectViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }
}