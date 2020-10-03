/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.Generic;
using System.Windows;

namespace ERDock.Controls
{
  internal interface IOverlayWindowHost
  {
    #region Properties

    DockingManager Manager
    {
      get;
    }

    #endregion

    #region Methods

    bool HitTest( Point dragPoint );

    IOverlayWindow ShowOverlayWindow( LayoutFloatingWindowControl draggingWindow );

    void HideOverlayWindow();

    IEnumerable<IDropArea> GetDropAreas( LayoutFloatingWindowControl draggingWindow );

    #endregion
  }
}
