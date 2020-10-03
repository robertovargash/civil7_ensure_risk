/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERComp.Core.Converters
{
  public class ObjectTypeToNameConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      if( value != null )
      {
        if( value is Type )
        {
          var displayNameAttribute = ( ( Type )value ).GetCustomAttributes( false ).OfType<DisplayNameAttribute>().FirstOrDefault();
          return ( displayNameAttribute != null ) ? displayNameAttribute.DisplayName : ( ( Type )value ).Name;
        }

        var type = value.GetType();
        var valueString = value.ToString();
        if( string.IsNullOrEmpty( valueString )
         || ( valueString == type.UnderlyingSystemType.ToString() ) )
        {
          var displayNameAttribute = type.GetCustomAttributes( false ).OfType<DisplayNameAttribute>().FirstOrDefault();
          return ( displayNameAttribute != null ) ? displayNameAttribute.DisplayName : type.Name;
        }

        return value; 
      }
      return null;
    }
    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
