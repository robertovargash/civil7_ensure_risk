﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ERComp.Core.Utilities;

namespace ERComp.Panels
{
  public static class SwitchTemplate
  {
    #region ID Attached Property

    public static readonly DependencyProperty IDProperty =
      DependencyProperty.RegisterAttached( "ID", typeof( string ), typeof( SwitchTemplate ),
        new FrameworkPropertyMetadata( null, 
          new PropertyChangedCallback( SwitchTemplate.OnIDChanged ) ) );

    public static string GetID( DependencyObject d )
    {
      return ( string )d.GetValue( SwitchTemplate.IDProperty );
    }

    public static void SetID( DependencyObject d, string value )
    {
      d.SetValue( SwitchTemplate.IDProperty, value );
    }

    private static void OnIDChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      if( ( e.NewValue == null ) || !( d is UIElement ) )
        return;

      SwitchPresenter parentPresenter = VisualTreeHelperEx.FindAncestorByType<SwitchPresenter>( d );
      if( parentPresenter != null )
      {
        parentPresenter.RegisterID( e.NewValue as string, d as FrameworkElement );
      }
      else
      {
        d.Dispatcher.BeginInvoke( DispatcherPriority.Loaded,
            ( ThreadStart )delegate()
        {
          parentPresenter = VisualTreeHelperEx.FindAncestorByType<SwitchPresenter>( d );
          if( parentPresenter != null )
          {
            parentPresenter.RegisterID( e.NewValue as string, d as FrameworkElement );
          }
        } );
      }
    }

    #endregion
  }
}
