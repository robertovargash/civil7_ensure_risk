/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  public abstract class OverlayArea : IOverlayWindowArea
  {
    #region Members

    private IOverlayWindow _overlayWindow;
    private Rect? _screenDetectionArea;

    #endregion

    #region Constructors

    internal OverlayArea( IOverlayWindow overlayWindow )
    {
      _overlayWindow = overlayWindow;
    }

    #endregion

    #region Internal Methods

    protected void SetScreenDetectionArea( Rect rect )
    {
      _screenDetectionArea = rect;
    }

    #endregion

    #region IOverlayWindowArea

    Rect IOverlayWindowArea.ScreenDetectionArea
    {
      get
      {
        return _screenDetectionArea.Value;
      }
    }

    #endregion
  }
}
