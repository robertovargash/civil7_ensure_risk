/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.ObjectModel;

namespace ERDock.Layout
{
  public interface ILayoutRoot
  {
    DockingManager Manager
    {
      get;
    }

    LayoutPanel RootPanel
    {
      get;
    }

    LayoutAnchorSide TopSide
    {
      get;
    }
    LayoutAnchorSide LeftSide
    {
      get;
    }
    LayoutAnchorSide RightSide
    {
      get;
    }
    LayoutAnchorSide BottomSide
    {
      get;
    }

    LayoutContent ActiveContent
    {
      get; set;
    }

    ObservableCollection<LayoutFloatingWindow> FloatingWindows
    {
      get;
    }
    ObservableCollection<LayoutAnchorable> Hidden
    {
      get;
    }

    void CollectGarbage();
  }
}
