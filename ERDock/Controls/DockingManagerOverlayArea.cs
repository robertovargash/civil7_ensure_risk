/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  public class DockingManagerOverlayArea : OverlayArea
  {
    #region Members

    private DockingManager _manager;

    #endregion

    #region Constructors

    internal DockingManagerOverlayArea( IOverlayWindow overlayWindow, DockingManager manager )
        : base( overlayWindow )
    {
      _manager = manager;

      base.SetScreenDetectionArea( new Rect(
          _manager.PointToScreenDPI( new Point() ),
          _manager.TransformActualSizeToAncestor() ) );
    }

    #endregion
  }
}
