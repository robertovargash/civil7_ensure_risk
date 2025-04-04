﻿#pragma checksum "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "45984A52157DF8B1EBF5C5D1152C75B2AD1923CE766AF13B6D28470EF364EACA"
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
    /// SetConnectionWindow
    /// </summary>
    public partial class SetConnectionWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSet;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnClose;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextServer;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.DialogHost ErrorMessageDialog;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
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
            System.Uri resourceLocater = new System.Uri("/EnsureRisk;component/windows/settings/setconnectionwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
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
            this.BtnSet = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
            this.BtnSet.Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BtnClose = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
            this.BtnClose.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\..\Windows\Settings\SetConnectionWindow.xaml"
            this.BtnClose.Loaded += new System.Windows.RoutedEventHandler(this.BtnClose_Loaded);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TextServer = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.ErrorMessageDialog = ((MaterialDesignThemes.Wpf.DialogHost)(target));
            return;
            case 5:
            this.TextMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

