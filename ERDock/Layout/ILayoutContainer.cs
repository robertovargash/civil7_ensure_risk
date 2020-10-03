/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.Generic;

namespace ERDock.Layout
{
  public interface ILayoutContainer : ILayoutElement
  {
    #region Properties

    IEnumerable<ILayoutElement> Children
    {
      get;
    }

    int ChildrenCount
    {
      get;
    }

    #endregion

    #region Methods

    void RemoveChild( ILayoutElement element );

    void ReplaceChild( ILayoutElement oldElement, ILayoutElement newElement );

    #endregion    
  }
}
