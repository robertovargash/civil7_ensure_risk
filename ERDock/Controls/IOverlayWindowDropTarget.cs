/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  interface IOverlayWindowDropTarget
  {
    Rect ScreenDetectionArea
    {
      get;
    }

    OverlayWindowDropTargetType Type
    {
      get;
    }
  }
}
