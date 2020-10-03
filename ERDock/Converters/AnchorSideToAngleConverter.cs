/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows.Data;
using ERDock.Layout;

namespace ERDock.Converters
{
  [ValueConversion( typeof( AnchorSide ), typeof( double ) )]
  public class AnchorSideToAngleConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      AnchorSide side = ( AnchorSide )value;
      if( side == AnchorSide.Left ||
          side == AnchorSide.Right )
        return 90.0;

      return Binding.DoNothing;
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
