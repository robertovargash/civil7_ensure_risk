/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

namespace ERDock.Layout
{
  public interface ILayoutPane : ILayoutContainer, ILayoutElementWithVisibility
  {
    void MoveChild( int oldIndex, int newIndex );

    void RemoveChildAt( int childIndex );
  }
}
