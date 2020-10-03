/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;

namespace ERDock.Layout
{
  public interface ILayoutOrientableGroup : ILayoutGroup
  {
    Orientation Orientation
    {
      get; set;
    }
  }
}
