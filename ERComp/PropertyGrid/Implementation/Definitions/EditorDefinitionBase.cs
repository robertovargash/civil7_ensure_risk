﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace ERComp.PropertyGrid
{
  public abstract class EditorDefinitionBase : PropertyDefinitionBase
  {

    internal EditorDefinitionBase() { }

    internal FrameworkElement GenerateEditingElementInternal( PropertyItemBase propertyItem ) 
    {
      return this.GenerateEditingElement( propertyItem );
    }

    protected virtual FrameworkElement GenerateEditingElement( PropertyItemBase propertyItem ) { return null; }

    internal void UpdateProperty( FrameworkElement element, DependencyProperty elementProp, DependencyProperty definitionProperty )
    {
      object currentValue = this.GetValue( definitionProperty );
      object localValue = this.ReadLocalValue( definitionProperty );
      object elementValue = element.GetValue( elementProp );
      bool areEquals = false;

      // Avoid setting values if it does not affect anything 
      // because setting a local value may prevent a style setter from being active.
      if( localValue != DependencyProperty.UnsetValue )
      {
        if( ( elementValue != null ) && ( currentValue != null ) )
        {
          areEquals = ( elementValue.GetType().IsValueType && currentValue.GetType().IsValueType )
                      ? elementValue.Equals( currentValue )  // Value Types
                      : currentValue == element.GetValue( elementProp ); // Reference Types
        }

        if( !areEquals )
        {
          element.SetValue( elementProp, currentValue );
        }
        else
        {
          element.ClearValue( elementProp );
        }
      }
    }
  }
}
