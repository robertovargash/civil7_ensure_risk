using System;
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
}
