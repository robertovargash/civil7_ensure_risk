#pragma checksum "..\..\..\Classess\PaintLayoutDocument.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A14A0A8D3945AED9F2D8CB4FE094C188D0BDEBFCA23A9FC47648144EE305105E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EnsureRisk.Classess;
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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;


namespace EnsureRisk.Classess
{


    /// <summary>
    /// PaintLayoutDocument
    /// </summary>
    public partial class PaintLayoutDocument : Xceed.Wpf.AvalonDock.Layout.LayoutDocument, System.Windows.Markup.IComponentConnector
    {

#line default
#line hidden


#line 21 "..\..\..\Classess\PaintLayoutDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridBottomToolBar;

#line default
#line hidden


#line 45 "..\..\..\Classess\PaintLayoutDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnMinusZoom;

#line default
#line hidden


#line 46 "..\..\..\Classess\PaintLayoutDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderZoom;

#line default
#line hidden


#line 47 "..\..\..\Classess\PaintLayoutDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnPlusZoom;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EnsureRisk;component/classess/paintlayoutdocument.xaml", System.UriKind.Relative);

#line 1 "..\..\..\Classess\PaintLayoutDocument.xaml"
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
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.ScrollGridPaint = ((System.Windows.Controls.ScrollViewer)(target));
                    return;
                case 2:
                    this.CanvasMain = ((System.Windows.Controls.Grid)(target));
                    return;
                case 3:
                    this.GridPaintLines = ((System.Windows.Controls.Grid)(target));

#line 16 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.GridPaintLines.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.GridPaintLines_MouseWheel);

#line default
#line hidden

#line 16 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.GridPaintLines.MouseMove += new System.Windows.Input.MouseEventHandler(this.GridPaintLines_MouseMove);

#line default
#line hidden

#line 16 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.GridPaintLines.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.GridPaintLines_MouseDown);

#line default
#line hidden

#line 16 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.GridPaintLines.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.GridPaintLines_MouseUp);

#line default
#line hidden
                    return;
                case 4:
                    this.textChangeName = ((System.Windows.Controls.TextBox)(target));

#line 17 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.textChangeName.AddHandler(System.Windows.Input.Keyboard.KeyDownEvent, new System.Windows.Input.KeyEventHandler(this.TextChangeName_KeyDown));

#line default
#line hidden

#line 17 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.textChangeName.LostFocus += new System.Windows.RoutedEventHandler(this.TextChangeName_LostFocus);

#line default
#line hidden

#line 17 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.textChangeName.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.TextChangeName_IsVisibleChanged);

#line default
#line hidden
                    return;
                case 5:
                    this.GridBottomToolBar = ((System.Windows.Controls.Grid)(target));
                    return;
                case 6:
                    this.Progress1 = ((System.Windows.Controls.ProgressBar)(target));
                    return;
                case 7:
                    this.cbFilterTopR = ((System.Windows.Controls.ComboBox)(target));

#line 34 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.cbFilterTopR.DropDownClosed += new System.EventHandler(this.CbFilterTopR_DropDownClosed);

#line default
#line hidden
                    return;
                case 8:
                    this.BtnMinusZoom = ((System.Windows.Controls.Button)(target));

#line 45 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.BtnMinusZoom.Click += new System.Windows.RoutedEventHandler(this.BtnMinusZoom_Click);

#line default
#line hidden
                    return;
                case 9:
                    this.sliderZoom = ((System.Windows.Controls.Slider)(target));

#line 46 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.sliderZoom.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.SliderZoom_ValueChanged);

#line default
#line hidden

#line 46 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.sliderZoom.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.SliderZoom_MouseWheel);

#line default
#line hidden
                    return;
                case 10:
                    this.BtnPlusZoom = ((System.Windows.Controls.Button)(target));

#line 47 "..\..\..\Classess\PaintLayoutDocument.xaml"
                    this.BtnPlusZoom.Click += new System.Windows.RoutedEventHandler(this.BtnPlusZoom_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal EnsureRisk.Classess.LGrid CanvasMain;
        internal EnsureRisk.Classess.GridPaint GridPaintLines;
        internal EnsureRisk.Classess.MyGrid TheTopGrid;
        internal EnsureRisk.Classess.MyGrid TheMainGrid;
        internal EnsureRisk.Classess.ScrollViewerDiagram ScrollGridPaint;
        internal EnsureRisk.Classess.LTextBox textChangeName;
        internal System.Windows.Controls.ProgressBar TheProgressBar;
        internal System.Windows.Controls.ComboBox CbFilterTopR;
    }
}
