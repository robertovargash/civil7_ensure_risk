/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Controls
{
  internal class WindowActivateEventArgs : EventArgs
  {
    #region Constructors

    public WindowActivateEventArgs( IntPtr hwndActivating )
    {
      HwndActivating = hwndActivating;
    }

    #endregion

    #region Properties

    public IntPtr HwndActivating
    {
      get;
      private set;
    }

    #endregion
  }
}
