using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnsureBusinesss.Business
{
    public class MyGroupButton:Button
    {
        public int IdRisk { get; set; }
        public int IdGroup { get; set; }

        public MyGroupButton() : base()
        {

        }
    }
}
