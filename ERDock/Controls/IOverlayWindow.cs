/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.Generic;

namespace ERDock.Controls
{
  internal interface IOverlayWindow
  {
    IEnumerable<IDropTarget> GetTargets();

    void DragEnter( LayoutFloatingWindowControl floatingWindow );
    void DragLeave( LayoutFloatingWindowControl floatingWindow );

    void DragEnter( IDropArea area );
    void DragLeave( IDropArea area );

    void DragEnter( IDropTarget target );
    void DragLeave( IDropTarget target );
    void DragDrop( IDropTarget target );
  }
}
