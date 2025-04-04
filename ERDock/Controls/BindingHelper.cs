﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

namespace ERDock.Controls
{
  internal class BindingHelper
  {
    #region Methods

    public static void RebindInactiveBindings( DependencyObject dependencyObject )
    {
      foreach( PropertyDescriptor property in TypeDescriptor.GetProperties( dependencyObject.GetType() ) )
      {
        var dpd = DependencyPropertyDescriptor.FromProperty( property );
        if( dpd != null )
        {
          BindingExpressionBase binding = BindingOperations.GetBindingExpressionBase( dependencyObject, dpd.DependencyProperty );
          if( binding != null )
          {
            //if (property.Name == "DataContext" || binding.HasError || binding.Status != BindingStatus.Active)
            {
              // Ensure that no pending calls are in the dispatcher queue
              Dispatcher.CurrentDispatcher.BeginInvoke( DispatcherPriority.SystemIdle, ( Action )delegate
                 {
                              // Remove and add the binding to re-trigger the binding error
                              dependencyObject.ClearValue( dpd.DependencyProperty );
                   BindingOperations.SetBinding( dependencyObject, dpd.DependencyProperty, binding.ParentBindingBase );
                 } );
            }
          }
        }
      }
    }

    #endregion
  }
}
