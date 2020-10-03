/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using ERDock.Layout;

namespace ERDock
{
  public class DocumentClosedEventArgs : EventArgs
  {
    public DocumentClosedEventArgs( LayoutDocument document )
    {
      Document = document;
    }

    public LayoutDocument Document
    {
      get;
      private set;
    }
  }
}
