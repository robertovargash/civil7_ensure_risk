/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using System.Windows.Media;
using ERDock.Layout;

namespace ERDock.Controls
{
  internal interface IDropTarget
  {
    #region Properties

    DropTargetType Type
    {
      get;
    }

    #endregion

    #region Methods

    Geometry GetPreviewPath( OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow );

    bool HitTest( Point dragPoint );

    void Drop( LayoutFloatingWindow floatingWindow );

    void DragEnter();

    void DragLeave();

    #endregion
  }
}
