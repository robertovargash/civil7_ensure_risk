/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ERComp.PropertyGrid.Converters;
using System.Windows;
using ERComp.Core.Utilities;
using System.Linq.Expressions;

namespace ERComp.PropertyGrid
{
  public abstract class DefinitionBase : DependencyObject
  {
    private bool _isLocked;

    internal bool IsLocked
    {
      get { return _isLocked; }
    }

    internal void ThrowIfLocked<TMember>( Expression<Func<TMember>> propertyExpression )
    {
      //In XAML, when using any properties of PropertyDefinition, the error of ThrowIfLocked is always thrown => prevent it !
      if( DesignerProperties.GetIsInDesignMode( this ) )
        return;

      if( this.IsLocked )
      {
        string propertyName = ReflectionHelper.GetPropertyOrFieldName( propertyExpression );
        string message = string.Format(
            @"Cannot modify {0} once the definition has beed added to a collection.",
            propertyName );
        throw new InvalidOperationException( message );
      }
    }

    internal virtual void Lock()
    {
      if( !_isLocked )
      {
        _isLocked = true;
      }
    }
  }
}
