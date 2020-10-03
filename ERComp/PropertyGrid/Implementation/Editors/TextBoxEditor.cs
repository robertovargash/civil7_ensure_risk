/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;
using System.Windows;
#if !VS2008
using System.ComponentModel.DataAnnotations;
#endif

namespace ERComp.PropertyGrid.Editors
{
  public class TextBoxEditor : TypeEditor<WatermarkTextBox>
  {
    protected override WatermarkTextBox CreateEditor()
    {
      return new PropertyGridEditorTextBox();
    }

#if !VS2008
    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>( propertyItem.PropertyDescriptor );
      if( displayAttribute != null )
      {
        this.Editor.Watermark = displayAttribute.GetPrompt();
      }
    }
#endif

    protected override void SetValueDependencyProperty()
    {
      ValueProperty = TextBox.TextProperty;
    }
  }

  public class PropertyGridEditorTextBox : WatermarkTextBox
  {
    static PropertyGridEditorTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorTextBox ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorTextBox ) ) );
    }
  }
}
