﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERComp.Core.Converters
{
  public class ThicknessSideRemovalConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      var thickness = (Thickness)value;
      var sideToRemove = int.Parse( (string)parameter );
      switch( sideToRemove )
      {
        case 0: thickness.Left = 0d; break;
        case 1: thickness.Top = 0d; break;
        case 2: thickness.Right = 0d; break;
        case 3: thickness.Bottom = 0d; break;
        default: throw new InvalidContentException("parameter should be from 0 to 3 to specify the side to remove.");
      }
      return thickness;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
