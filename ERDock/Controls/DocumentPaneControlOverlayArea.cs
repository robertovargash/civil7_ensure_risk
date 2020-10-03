/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  public class DocumentPaneControlOverlayArea : OverlayArea
  {
    #region Members

    private LayoutDocumentPaneControl _documentPaneControl;

    #endregion

    #region Constructors

    internal DocumentPaneControlOverlayArea(
        IOverlayWindow overlayWindow,
        LayoutDocumentPaneControl documentPaneControl )
        : base( overlayWindow )
    {
      _documentPaneControl = documentPaneControl;
      base.SetScreenDetectionArea( new Rect(  _documentPaneControl.PointToScreenDPI( new Point() ),  _documentPaneControl.TransformActualSizeToAncestor() ) );
    }

    #endregion
  }
}
