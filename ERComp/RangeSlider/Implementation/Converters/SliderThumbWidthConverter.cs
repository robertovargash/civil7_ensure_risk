/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ERComp.Converters
{
  [Obsolete("This class is no longer used internaly and may be removed in a future release")]
  public class SliderThumbWidthConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if( value is Slider )
      {
        string param = parameter.ToString();
        if( param == "0" )
          return RangeSlider.GetThumbWidth( ( Slider )value );
        else if( param == "1" )
          return RangeSlider.GetThumbHeight( ( Slider )value );
      }
      return 0d;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
