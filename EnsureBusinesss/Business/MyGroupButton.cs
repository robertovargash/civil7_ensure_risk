using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnsureBusinesss.Business
{
    public class MyGroupButton:Button
    {
        public decimal IdRisk { get; set; }
        public decimal IdGroup { get; set; }

        public MyGroupButton() : base()
        {

        }
    }

    public class MyTreeItemGroup : TreeViewItem
    {

        public static readonly DependencyProperty IdGroupProperty =
  DependencyProperty.Register("IdGroup", typeof(decimal), typeof(MyTreeItemGroup), new PropertyMetadata(default(decimal)));


        public decimal IdGroup
        {
            get { return (decimal)GetValue(IdGroupProperty); }
            set { SetValue(IdGroupProperty, value); }
        }

        public static readonly DependencyProperty dataViewProperty =
  DependencyProperty.Register("MyDV", typeof(DataView), typeof(MyTreeItemGroup), new PropertyMetadata(default(DataView)));

        public DataView MyDV
        {
            get { return (DataView)GetValue(dataViewProperty); }
            set { SetValue(dataViewProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
           "GroupName", typeof(string), typeof(MyTreeItemGroup), new PropertyMetadata(default(string)));

        public string GroupName
        {
            get { return (string)GetValue(IdGroupProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public MyTreeItemGroup() : base()
        {

        }
    }
}
