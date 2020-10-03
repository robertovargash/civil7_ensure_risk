/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace ERDock.Converters
{
  public class UriSourceToBitmapImageConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      if( value == null )
        return Binding.DoNothing;
      //return (Uri)value;
      return new Image() { Source = new BitmapImage( ( Uri )value ) };
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
