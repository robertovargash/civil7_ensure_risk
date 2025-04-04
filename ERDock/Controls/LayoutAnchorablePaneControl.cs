﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using ERDock.Layout;

namespace ERDock.Controls
{
  public class LayoutAnchorablePaneControl : TabControl, ILayoutControl//, ILogicalChildrenContainer
  {
    #region Members

    private LayoutAnchorablePane _model;

    #endregion

    #region Constructors

    static LayoutAnchorablePaneControl()
    {
      FocusableProperty.OverrideMetadata( typeof( LayoutAnchorablePaneControl ), new FrameworkPropertyMetadata( false ) );
    }

    public LayoutAnchorablePaneControl( LayoutAnchorablePane model )
    {
      if( model == null )
        throw new ArgumentNullException( "model" );

      _model = model;

      SetBinding( ItemsSourceProperty, new Binding( "Model.Children" ) { Source = this } );
      SetBinding( FlowDirectionProperty, new Binding( "Model.Root.Manager.FlowDirection" ) { Source = this } );

      this.LayoutUpdated += new EventHandler( OnLayoutUpdated );
    }

    #endregion

    #region Properties

    public ILayoutElement Model
    {
      get
      {
        return _model;
      }
    }

    #endregion

    #region Overrides

    protected override void OnGotKeyboardFocus( System.Windows.Input.KeyboardFocusChangedEventArgs e )
    {
      if( ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }

      base.OnGotKeyboardFocus( e );
    }

    protected override void OnMouseLeftButtonDown( System.Windows.Input.MouseButtonEventArgs e )
    {
      base.OnMouseLeftButtonDown( e );

      if( !e.Handled && ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }
    }

    protected override void OnMouseRightButtonDown( System.Windows.Input.MouseButtonEventArgs e )
    {
      base.OnMouseRightButtonDown( e );

      if( !e.Handled && ( _model != null ) && ( _model.SelectedContent != null ) )
      {
        _model.SelectedContent.IsActive = true;
      }
    }


    #endregion

    #region Private Methods

    private void OnLayoutUpdated( object sender, EventArgs e )
    {
      var modelWithAtcualSize = _model as ILayoutPositionableElementWithActualSize;
      modelWithAtcualSize.ActualWidth = ActualWidth;
      modelWithAtcualSize.ActualHeight = ActualHeight;
    }

    #endregion
  }
}
