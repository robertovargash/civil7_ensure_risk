/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using ERComp;
namespace ERComp.PropertyGrid.Editors
{
  public class ColorEditor : TypeEditor<ColorPicker>
  {
    protected override ColorPicker CreateEditor()
    {
      return new PropertyGridEditorColorPicker();
    }

    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      Editor.BorderThickness = new System.Windows.Thickness( 0 );
      Editor.DisplayColorAndName = true;
    }
    protected override void SetValueDependencyProperty()
    {
      ValueProperty = ColorPicker.SelectedColorProperty;
    }
  }

  public class PropertyGridEditorColorPicker : ColorPicker
  {
    static PropertyGridEditorColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorColorPicker ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorColorPicker ) ) );
    }
  }
}
