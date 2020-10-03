/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using ERDock.Layout;

namespace ERDock
{
  class LayoutEventArgs : EventArgs
  {
    public LayoutEventArgs( LayoutRoot layoutRoot )
    {
      LayoutRoot = layoutRoot;
    }

    public LayoutRoot LayoutRoot
    {
      get;
      private set;
    }
  }
}
