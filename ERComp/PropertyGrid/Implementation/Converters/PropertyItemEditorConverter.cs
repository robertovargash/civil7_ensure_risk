﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ERComp.PropertyGrid.Converters
{
  public class PropertyItemEditorConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if( ( values == null ) || ( values.Length != 2 ) )
        return null;

      var editor = values[ 0 ];
      var isReadOnly = values[ 1 ] as bool?;

      if( ( editor == null ) || !isReadOnly.HasValue )
        return editor;

      // Get Editor.IsReadOnly
      var editorType = editor.GetType();
      var editorIsReadOnlyPropertyInfo = editorType.GetProperty( "IsReadOnly" );
      if( editorIsReadOnlyPropertyInfo != null )
      {
        if( !this.IsPropertySetLocally( editor, TextBoxBase.IsReadOnlyProperty )  )
        {
          // Set Editor.IsReadOnly to PropertyGrid.IsReadOnly.
          editorIsReadOnlyPropertyInfo.SetValue( editor, isReadOnly, null );
        }
      }
      // No Editor.IsReadOnly property, set the Editor.IsEnabled property.
      else
      {
        var editorIsEnabledPropertyInfo = editorType.GetProperty( "IsEnabled" );
        if( editorIsEnabledPropertyInfo != null )
        {
          if( !this.IsPropertySetLocally( editor, UIElement.IsEnabledProperty ) )
          {
            // Set Editor.IsEnabled to !PropertyGrid.IsReadOnly.
            editorIsEnabledPropertyInfo.SetValue( editor, !isReadOnly, null );
          }
        }
      }

      return editor;
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }

    private bool IsPropertySetLocally( object editor, DependencyProperty dp )
    {
      if( dp == null )
        return false;

      var editorObject = editor as DependencyObject;
      if( editorObject == null )
        return false;

      var valueSource = DependencyPropertyHelper.GetValueSource( editorObject, dp );
      if( valueSource == null )
        return false;

      return ( valueSource.BaseValueSource == BaseValueSource.Local );
    }
  }
}
