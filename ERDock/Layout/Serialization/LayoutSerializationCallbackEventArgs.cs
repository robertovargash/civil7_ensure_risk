/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.ComponentModel;

namespace ERDock.Layout.Serialization
{
  public class LayoutSerializationCallbackEventArgs : CancelEventArgs
  {
    #region constructor

    public LayoutSerializationCallbackEventArgs( LayoutContent model, object previousContent )
    {
      Cancel = false;
      Model = model;
      Content = previousContent;
    }

    #endregion

    #region Properties

    public LayoutContent Model
    {
      get; private set;
    }

    public object Content
    {
      get; set;
    }

    #endregion
  }
}
