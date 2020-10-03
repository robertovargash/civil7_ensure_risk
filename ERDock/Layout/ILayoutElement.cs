/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.ComponentModel;

namespace ERDock.Layout
{
  public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
  {
    ILayoutContainer Parent
    {
      get;
    }
    ILayoutRoot Root
    {
      get;
    }
  }
}
