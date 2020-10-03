/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ERComp.Core.Converters
{
  public class HalfConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      double size = ( double )value;
      double modifier = (parameter != null) ? double.Parse( ( string )parameter ) : 0d;
      if( modifier != 0 )
        return Math.Max(0, size - modifier) / 2;

      return ( size / 2 );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
