﻿#pragma checksum "..\..\..\Windows\WindowSingleSelection.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A4479267416C64C8EAB760BC7BCD38DA13DEA6C32B18F907EC435D913C2F81E3"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using EnsureRisk;
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
    /// WindowSingleSelection
    /// </summary>
    public partial class WindowSingleSelection : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 30 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnFilter;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtFilterRisk;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClearFilter;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgSelection;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTemplateColumn Selectbtn;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSelect;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnCancel;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.DialogHost ErrorMessageDialog;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\Windows\WindowSingleSelection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextMessage;
        
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
            System.Uri resourceLocater = new System.Uri("/EnsureRisk;component/windows/windowsingleselection.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\WindowSingleSelection.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 12 "..\..\..\Windows\WindowSingleSelection.xaml"
            ((EnsureRisk.Windows.WindowSingleSelection)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnFilter = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.btnFilter.Click += new System.Windows.RoutedEventHandler(this.btnFilter_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtFilterRisk = ((System.Windows.Controls.TextBox)(target));
            
            #line 33 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.txtFilterRisk.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TxtFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnClearFilter = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.btnClearFilter.Click += new System.Windows.RoutedEventHandler(this.btnClearFilter_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.dgSelection = ((System.Windows.Controls.DataGrid)(target));
            
            #line 40 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.dgSelection.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.dgSelection_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 40 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.dgSelection.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.dgSelection_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Selectbtn = ((System.Windows.Controls.DataGridTemplateColumn)(target));
            return;
            case 9:
            this.BtnSelect = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.BtnSelect.Click += new System.Windows.RoutedEventHandler(this.BtnSelect_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.BtnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 56 "..\..\..\Windows\WindowSingleSelection.xaml"
            this.BtnCancel.Click += new System.Windows.RoutedEventHandler(this.BtnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.ErrorMessageDialog = ((MaterialDesignThemes.Wpf.DialogHost)(target));
            return;
            case 12:
            this.TextMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 7:
            
            #line 47 "..\..\..\Windows\WindowSingleSelection.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_UnSelect);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 48 "..\..\..\Windows\WindowSingleSelection.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Select);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

