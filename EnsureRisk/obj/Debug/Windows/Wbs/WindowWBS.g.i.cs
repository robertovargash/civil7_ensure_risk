﻿#pragma checksum "..\..\..\..\Windows\Wbs\WindowWBS.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E50980BD61A3FF2FBE791A5CE0BB0FDA3BE6357B1057203DF0ACB6195D534C4D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using DataMapping.Data;
using EnsureRisk.Classess;
using EnsureRisk.Windows;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EnsureRisk.Windows {
    
    
    /// <summary>
    /// WindowWBS
    /// </summary>
    public partial class WindowWBS : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextLevel;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbUser;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextName;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkIsManager;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnAdd;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnDel;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgWBS;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn Level;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn WBS;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn UserName;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnOK;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnCancel;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.DialogHost ErrorMessageDialog;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextMessage;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.DialogHost YesNoDialog;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextYesNoMessage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EnsureRisk;component/windows/wbs/windowwbs.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 16 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            ((EnsureRisk.Windows.WindowWBS)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TextLevel = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.TextLevel.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextLevel_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cbUser = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.TextName = ((System.Windows.Controls.TextBox)(target));
            
            #line 41 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.TextName.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextName_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.chkIsManager = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.BtnAdd = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.BtnAdd.Click += new System.Windows.RoutedEventHandler(this.BtnAdd_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnDel = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.BtnDel.Click += new System.Windows.RoutedEventHandler(this.BtnDel_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.dgWBS = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.Level = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 10:
            this.WBS = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 11:
            this.UserName = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 12:
            this.BtnOK = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.BtnOK.Click += new System.Windows.RoutedEventHandler(this.BtnOK_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.BtnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.BtnCancel.Click += new System.Windows.RoutedEventHandler(this.BtnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.ErrorMessageDialog = ((MaterialDesignThemes.Wpf.DialogHost)(target));
            return;
            case 15:
            this.TextMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 16:
            this.YesNoDialog = ((MaterialDesignThemes.Wpf.DialogHost)(target));
            
            #line 86 "..\..\..\..\Windows\Wbs\WindowWBS.xaml"
            this.YesNoDialog.DialogClosing += new MaterialDesignThemes.Wpf.DialogClosingEventHandler(this.YesNoDialog_DialogClosing);
            
            #line default
            #line hidden
            return;
            case 17:
            this.TextYesNoMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

