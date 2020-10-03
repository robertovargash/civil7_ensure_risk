/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

namespace ERDock.Layout
{
  public interface ILayoutContentSelector
  {
    #region Properties

    int SelectedContentIndex
    {
      get; set;
    }

    LayoutContent SelectedContent
    {
      get;
    }

    #endregion

    #region Methods

    int IndexOf( LayoutContent content );

    #endregion
  }
}
