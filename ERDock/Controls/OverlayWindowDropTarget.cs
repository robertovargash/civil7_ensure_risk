/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  public class OverlayWindowDropTarget : IOverlayWindowDropTarget
  {
    #region Members

    private IOverlayWindowArea _overlayArea;
    private Rect _screenDetectionArea;
    private OverlayWindowDropTargetType _type;

    #endregion

    #region Constructors

    internal OverlayWindowDropTarget( IOverlayWindowArea overlayArea, OverlayWindowDropTargetType targetType, FrameworkElement element )
    {
      _overlayArea = overlayArea;
      _type = targetType;
      _screenDetectionArea = new Rect( element.TransformToDeviceDPI( new Point() ), element.TransformActualSizeToAncestor() );
    }

    #endregion


    #region IOverlayWindowDropTarget

    Rect IOverlayWindowDropTarget.ScreenDetectionArea
    {
      get
      {
        return _screenDetectionArea;
      }

    }

    OverlayWindowDropTargetType IOverlayWindowDropTarget.Type
    {
      get
      {
        return _type;
      }
    }

    #endregion
  }
}
