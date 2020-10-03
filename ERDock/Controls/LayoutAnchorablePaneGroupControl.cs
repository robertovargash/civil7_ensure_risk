/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;
using System.Windows;
using ERDock.Layout;

namespace ERDock.Controls
{
  public class LayoutAnchorablePaneGroupControl : LayoutGridControl<ILayoutAnchorablePane>, ILayoutControl
  {
    #region Members

    private LayoutAnchorablePaneGroup _model;

    #endregion

    #region Constructors

    internal LayoutAnchorablePaneGroupControl( LayoutAnchorablePaneGroup model )
        : base( model, model.Orientation )
    {
      _model = model;
    }

    #endregion

    #region Overrides

    protected override void OnFixChildrenDockLengths()
    {
      if( _model.Orientation == Orientation.Horizontal )
      {
        for( int i = 0; i < _model.Children.Count; i++ )
        {
          var childModel = _model.Children[ i ] as ILayoutPositionableElement;
          if( !childModel.DockWidth.IsStar )
          {
            childModel.DockWidth = new GridLength( 1.0, GridUnitType.Star );
          }
        }
      }
      else
      {
        for( int i = 0; i < _model.Children.Count; i++ )
        {
          var childModel = _model.Children[ i ] as ILayoutPositionableElement;
          if( !childModel.DockHeight.IsStar )
          {
            childModel.DockHeight = new GridLength( 1.0, GridUnitType.Star );
          }
        }
      }
    }

    #endregion
  }
}
