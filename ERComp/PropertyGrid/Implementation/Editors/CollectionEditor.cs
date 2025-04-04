﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ERComp.Core.Utilities;

namespace ERComp.PropertyGrid.Editors
{
  public class CollectionEditor : TypeEditor<CollectionControlButton>
  {
    protected override void SetValueDependencyProperty()
    {
      ValueProperty = CollectionControlButton.ItemsSourceProperty;
    }

    protected override CollectionControlButton CreateEditor()
    {
      return new PropertyGridEditorCollectionControl();
    }

    protected override void SetControlProperties( PropertyItem propertyItem )
    {

      var propertyGrid = propertyItem.ParentElement as PropertyGrid;
      if( propertyGrid != null )
      {
        // Use the PropertyGrid.EditorDefinitions for the CollectionControl's propertyGrid.
        this.Editor.EditorDefinitions = propertyGrid.EditorDefinitions;
      }
    }

    protected override void ResolveValueBinding( PropertyItem propertyItem )
    {
      var type = propertyItem.PropertyType;

      Editor.ItemsSourceType = type;

      if( type.BaseType == typeof( System.Array ) )
      {
        Editor.NewItemTypes = new List<Type>() { type.GetElementType() };
      }
      else 
      {
        if( (propertyItem.DescriptorDefinition != null)
            && (propertyItem.DescriptorDefinition.NewItemTypes != null)
            && (propertyItem.DescriptorDefinition.NewItemTypes.Count > 0) )
        {
          Editor.NewItemTypes = propertyItem.DescriptorDefinition.NewItemTypes;
        }
        else
        {
          //Check if we have a Dictionary
          var dictionaryTypes = ListUtilities.GetDictionaryItemsType( type );
          if( (dictionaryTypes != null) && (dictionaryTypes.Length == 2) )
          {
            // A Dictionary contains KeyValuePair that can't be edited.
            // We need to create EditableKeyValuePairs.
            // Create a EditableKeyValuePair< TKey, TValue> type from dictionary generic arguments type
            var editableKeyValuePairType = ListUtilities.CreateEditableKeyValuePairType( dictionaryTypes[ 0 ], dictionaryTypes[ 1 ] );
            Editor.NewItemTypes = new List<Type>() { editableKeyValuePairType };
          }
          else
          {
            //Check if we have a list
            var listType = ListUtilities.GetListItemType( type );
            if( listType != null  )
            {
              Editor.NewItemTypes = new List<Type>() { listType };
            }
            else
            {
              //Check if we have a Collection of T
              var colType = ListUtilities.GetCollectionItemType( type );
              if( colType != null )
              {
                Editor.NewItemTypes = new List<Type>() { colType };
              }
            }
          }
        }
      }

      base.ResolveValueBinding( propertyItem );
    }

  }

  public class PropertyGridEditorCollectionControl : CollectionControlButton
  {
    static PropertyGridEditorCollectionControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorCollectionControl ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorCollectionControl ) ) );
    }
  }
}
