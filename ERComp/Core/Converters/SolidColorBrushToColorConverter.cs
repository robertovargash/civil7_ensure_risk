﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ERComp.Core.Converters
{
  public class SolidColorBrushToColorConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      SolidColorBrush brush = value as SolidColorBrush;
      if( brush != null )
        return brush.Color;

      return default( Color? );
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      if( value != null )
      {
        Color color = ( Color )value;
        return new SolidColorBrush( color );
      }

      return default( SolidColorBrush );
    }

    #endregion
  }
}
