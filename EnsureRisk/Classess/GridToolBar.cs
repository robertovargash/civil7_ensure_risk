using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class GridToolBar : MyGrid
    {
        public ColumnDefinition Col0 { get; set; }
        public ColumnDefinition Col1 { get; set; }
        public ColumnDefinition Col2 { get; set; }
        public ColumnDefinition Col3 { get; set; }
        public ColumnDefinition Col4 { get; set; }
        public ColumnDefinition Col5 { get; set; }
        public ColumnDefinition Col6 { get; set; }

        public void SetTheColumn(UIElement element, int position)
        {
            SetColumn(element, position);
        }

        public GridToolBar() : base()
        {
            VerticalAlignment = VerticalAlignment.Bottom;
            Col0 = new ColumnDefinition() { };
            Col1 = new ColumnDefinition() { };
            Col2 = new ColumnDefinition() { Width = new GridLength(60) };
            Col3 = new ColumnDefinition() { Width = new GridLength(15) };
            Col4 = new ColumnDefinition() { Width = new GridLength(25) };
            Col5 = new ColumnDefinition() { Width = new GridLength(150) };
            Col6 = new ColumnDefinition() { Width = new GridLength(25) };
            ColumnDefinitions.Add(Col0);
            ColumnDefinitions.Add(Col1);
            ColumnDefinitions.Add(Col2);
            ColumnDefinitions.Add(Col3);
            ColumnDefinitions.Add(Col4);
            ColumnDefinitions.Add(Col5);
            ColumnDefinitions.Add(Col6);
        }
    }
}
