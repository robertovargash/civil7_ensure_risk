﻿#pragma checksum "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A3EA716C3CA4578A8FB395FB1C266DB30FDC5EB07DF8950C416BBBFEE7C15FF1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// WindowSelectTopRisk
    /// </summary>
    public partial class WindowSelectTopRisk : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextTopRisk;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextUnit;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSelect;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnOK;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/EnsureRisk;component/windows/damages/windowselecttoprisk.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
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
            
            #line 15 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
            ((EnsureRisk.Windows.WindowSelectTopRisk)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TextTopRisk = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.TextUnit = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.BtnSelect = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
            this.BtnSelect.Click += new System.Windows.RoutedEventHandler(this.BtnSelect_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BtnOK = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
            this.BtnOK.Click += new System.Windows.RoutedEventHandler(this.BtnOK_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\Windows\Damages\WindowSelectTopRisk.xaml"
            this.BtnCancel.Click += new System.Windows.RoutedEventHandler(this.BtnCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
