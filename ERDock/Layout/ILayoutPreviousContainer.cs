/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

namespace ERDock.Layout
{
  interface ILayoutPreviousContainer
  {
    ILayoutContainer PreviousContainer
    {
      get; set;
    }

    string PreviousContainerId
    {
      get; set;
    }
  }
}
