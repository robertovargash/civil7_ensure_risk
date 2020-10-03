/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.Generic;
using System.Collections;
using System.Windows;

namespace ERComp.PropertyGrid.Editors
{
  public abstract class ComboBoxEditor : TypeEditor<System.Windows.Controls.ComboBox>
  {
    protected override void SetValueDependencyProperty()
    {
      ValueProperty = System.Windows.Controls.ComboBox.SelectedItemProperty;
    }

    protected override System.Windows.Controls.ComboBox CreateEditor()
    {
      return new PropertyGridEditorComboBox();
    }

    protected override void ResolveValueBinding( PropertyItem propertyItem )
    {
      SetItemsSource( propertyItem );
      base.ResolveValueBinding( propertyItem );
    }

    protected abstract IEnumerable CreateItemsSource( PropertyItem propertyItem );

    private void SetItemsSource( PropertyItem propertyItem )
    {
      Editor.ItemsSource = CreateItemsSource( propertyItem );
    }
  }

  public class PropertyGridEditorComboBox : System.Windows.Controls.ComboBox
  {
    static PropertyGridEditorComboBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorComboBox ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorComboBox ) ) );
    }
  }
}
