/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

namespace ERDock.Layout
{
  public interface ILayoutUpdateStrategy
  {
    bool BeforeInsertAnchorable(
        LayoutRoot layout,
        LayoutAnchorable anchorableToShow,
        ILayoutContainer destinationContainer );

    void AfterInsertAnchorable(
        LayoutRoot layout,
        LayoutAnchorable anchorableShown );


    bool BeforeInsertDocument(
        LayoutRoot layout,
        LayoutDocument anchorableToShow,
        ILayoutContainer destinationContainer );

    void AfterInsertDocument(
        LayoutRoot layout,
        LayoutDocument anchorableShown );
  }
}
