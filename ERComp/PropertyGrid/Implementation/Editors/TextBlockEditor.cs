/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;
using System.Windows;

namespace ERComp.PropertyGrid.Editors
{
  public class TextBlockEditor : TypeEditor<TextBlock>
  {
    protected override TextBlock CreateEditor()
    {
      return new PropertyGridEditorTextBlock();
    }

    protected override void SetValueDependencyProperty()
    {
      ValueProperty = TextBlock.TextProperty;
    }

    protected override void SetControlProperties( PropertyItem propertyItem )
    {
      Editor.Margin = new System.Windows.Thickness( 5, 0, 0, 0 );
      Editor.TextTrimming = TextTrimming.CharacterEllipsis;
    }
  }

  public class PropertyGridEditorTextBlock : TextBlock
  {
    static PropertyGridEditorTextBlock()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( PropertyGridEditorTextBlock ), new FrameworkPropertyMetadata( typeof( PropertyGridEditorTextBlock ) ) );
    }
  }
}
