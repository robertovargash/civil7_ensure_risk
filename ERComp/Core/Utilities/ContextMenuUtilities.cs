﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using System.Windows.Media;
using ERComp.PropertyGrid;

namespace ERComp.Core.Utilities
{
  public class ContextMenuUtilities
  {
    public static readonly DependencyProperty OpenOnMouseLeftButtonClickProperty = DependencyProperty.RegisterAttached( "OpenOnMouseLeftButtonClick", typeof( bool ), typeof( ContextMenuUtilities ), new FrameworkPropertyMetadata( false, OpenOnMouseLeftButtonClickChanged ) );
    public static void SetOpenOnMouseLeftButtonClick( FrameworkElement element, bool value )
    {
      element.SetValue( OpenOnMouseLeftButtonClickProperty, value );
    }
    public static bool GetOpenOnMouseLeftButtonClick( FrameworkElement element )
    {
      return ( bool )element.GetValue( OpenOnMouseLeftButtonClickProperty );
    }

    public static void OpenOnMouseLeftButtonClickChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
    {
      var control = sender as FrameworkElement;
      if( control != null )
      {
        if( ( bool )e.NewValue )
        {
          control.PreviewMouseLeftButtonDown += ContextMenuUtilities.Control_PreviewMouseLeftButtonDown;
        }
        else
        {
          control.PreviewMouseLeftButtonDown -= ContextMenuUtilities.Control_PreviewMouseLeftButtonDown;
        }
      }
    }

    private static void Control_PreviewMouseLeftButtonDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      var control = sender as FrameworkElement;
      if( (control != null) && (control.ContextMenu != null) )
      {
        // Get PropertyItemBase parent
        var parent = VisualTreeHelper.GetParent( control );
        while( parent != null )
        {
          var propertyItemBase = parent as PropertyItemBase;
          if( propertyItemBase != null )
          {
            // Set the ContextMenu.DataContext to the PropertyItem associated to the clicked image.
            control.ContextMenu.DataContext = propertyItemBase;
            break;
          }
          parent = VisualTreeHelper.GetParent( parent );
        }

        control.ContextMenu.PlacementTarget = control;
        control.ContextMenu.IsOpen = true;
      }
    }
  }
}
