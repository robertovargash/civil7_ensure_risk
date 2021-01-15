using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class WBSNodes : ObservableObject
    {
        private ObservableCollection<WBSNodes> mChildren;

        // Add all of the properties of a node here. In this example,
        // all we have is a name and whether we are expanded.
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _name;

        public decimal ID_WBS
        {
            get { return _ID_WBS; }
            set
            {
                if (_ID_WBS != value)
                {
                    _ID_WBS = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private decimal _ID_WBS;

        public bool IsEyedOff
        {
            get { return _IsEyedOff; }
            set
            {
                if (_IsEyedOff != value)
                {
                    _IsEyedOff = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _IsEyedOff;

        public bool CanEdit
        {
            get { return _CanEdit; }
            set
            {
                if (_CanEdit != value)
                {
                    _CanEdit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _CanEdit;

        public bool CanDelete
        {
            get { return _CanDelete; }
            set
            {
                if (_CanDelete != value)
                {
                    _CanDelete = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _CanDelete;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _isExpanded;

        // Children are required to use this in a TreeView
        public IList<WBSNodes> Children { get { return mChildren; } }

        // Parent is optional. Include if you need to climb the tree
        // from code. Not usually necessary.
        public WBSNodes Parent { get; private set; }

        public WBSNodes(WBSNodes parent = null)
        {
            mChildren = new ObservableCollection<WBSNodes>();
            IsExpanded = true;
            Parent = parent;
        }
    }    
}
