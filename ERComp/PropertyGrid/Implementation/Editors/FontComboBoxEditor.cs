/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using ERComp.Core.Utilities;

namespace ERComp.PropertyGrid.Editors
{
  public class FontComboBoxEditor : ComboBoxEditor
  {
    protected override IEnumerable CreateItemsSource( PropertyItem propertyItem )
    {
      if( propertyItem.PropertyType == typeof( FontFamily ) )
        return FontUtilities.Families.OrderBy( x => x.Source);
      else if( propertyItem.PropertyType == typeof( FontWeight ) )
        return FontUtilities.Weights;
      else if( propertyItem.PropertyType == typeof( FontStyle ) )
        return FontUtilities.Styles;
      else if( propertyItem.PropertyType == typeof( FontStretch ) )
        return FontUtilities.Stretches;

      return null;
    }
  }
}
