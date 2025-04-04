﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERComp.PropertyGrid.Editors
{
  public class PrimitiveTypeCollectionEditor : TypeEditor<PrimitiveTypeCollectionControl>
  {
    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      Editor.BorderThickness = new System.Windows.Thickness( 0 );
      Editor.Content = "(Collection)";
    }

    protected override void SetValueDependencyProperty()
    {
      ValueProperty = PrimitiveTypeCollectionControl.ItemsSourceProperty;
    }

    protected override PrimitiveTypeCollectionControl CreateEditor()
    {
      return new PropertyGridEditorPrimitiveTypeCollectionControl();
    }

    protected override void ResolveValueBinding( PropertyItem propertyItem )
    {
      var type = propertyItem.PropertyType;
      Editor.ItemsSourceType = type;

      if( type.BaseType == typeof( System.Array ) )
      {
        Editor.ItemType = type.GetElementType();
      }
      else
      {
        var typeArguments = type.GetGenericArguments();
        if( typeArguments.Length > 0 )
        {
          Editor.ItemType = typeArguments[ 0 ];
        }
      }

      base.ResolveValueBinding( propertyItem );
    }
  }

  public class PropertyGridEditorPrimitiveTypeCollectionControl : PrimitiveTypeCollectionControl
  {
    static PropertyGridEditorPrimitiveTypeCollectionControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorPrimitiveTypeCollectionControl ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorPrimitiveTypeCollectionControl ) ) );
    }
  }
}
