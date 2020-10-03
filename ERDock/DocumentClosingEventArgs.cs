/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.ComponentModel;
using ERDock.Layout;

namespace ERDock
{
  public class DocumentClosingEventArgs : CancelEventArgs
  {
    public DocumentClosingEventArgs( LayoutDocument document )
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
