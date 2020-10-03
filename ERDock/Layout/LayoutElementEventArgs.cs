/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Layout
{
  public class LayoutElementEventArgs : EventArgs
  {
    #region Constructors

    public LayoutElementEventArgs( LayoutElement element )
    {
      Element = element;
    }

    #endregion

    #region Properties

    public LayoutElement Element
    {
      get;
      private set;
    }

    #endregion
  }
}
