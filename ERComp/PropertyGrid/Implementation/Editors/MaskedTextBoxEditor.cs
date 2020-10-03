/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ERComp.PropertyGrid.Editors
{
  public class MaskedTextBoxEditor : TypeEditor<MaskedTextBox>
  {
    public string Mask
    {
      get;
      set;
    }

    public Type ValueDataType
    {
      get;
      set;
    }

    protected override MaskedTextBox CreateEditor()
    {
      return new PropertyGridEditorMaskedTextBox();
    }

    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      Editor.BorderThickness = new System.Windows.Thickness( 0 );
      this.Editor.ValueDataType = this.ValueDataType;
      this.Editor.Mask = this.Mask;
    }

    protected override void SetValueDependencyProperty()
    {
      this.ValueProperty = MaskedTextBox.ValueProperty;
    }
  }

  public class PropertyGridEditorMaskedTextBox : MaskedTextBox
  {
    static PropertyGridEditorMaskedTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorMaskedTextBox ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorMaskedTextBox ) ) );
    }
  }
}
