/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Layout
{
  internal interface ILayoutPositionableElement : ILayoutElement, ILayoutElementForFloatingWindow
  {
    GridLength DockWidth
    {
      get;
      set;
    }

    GridLength DockHeight
    {
      get;
      set;
    }

    double DockMinWidth
    {
      get; set;
    }
    double DockMinHeight
    {
      get; set;
    }

    bool AllowDuplicateContent
    {
      get; set;
    }

    bool IsVisible
    {
      get;
    }
  }


  internal interface ILayoutPositionableElementWithActualSize
  {
    double ActualWidth
    {
      get; set;
    }
    double ActualHeight
    {
      get; set;
    }
  }

  internal interface ILayoutElementForFloatingWindow
  {
    double FloatingWidth
    {
      get; set;
    }
    double FloatingHeight
    {
      get; set;
    }
    double FloatingLeft
    {
      get; set;
    }
    double FloatingTop
    {
      get; set;
    }
    bool IsMaximized
    {
      get; set;
    }
  }
}
