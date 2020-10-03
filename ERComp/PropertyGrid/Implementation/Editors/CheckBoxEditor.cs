/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using System.Windows.Controls;

namespace ERComp.PropertyGrid.Editors
{
  public class CheckBoxEditor : TypeEditor<CheckBox>
  {
    protected override CheckBox CreateEditor()
    {
      return new PropertyGridEditorCheckBox();
    }

    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      Editor.Margin = new Thickness( 5, 0, 0, 0 );
    }

    protected override void SetValueDependencyProperty()
    {
      ValueProperty = CheckBox.IsCheckedProperty;
    }
  }

  public class PropertyGridEditorCheckBox : CheckBox
  {
    static PropertyGridEditorCheckBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorCheckBox ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorCheckBox ) ) );
    }
  }
}
