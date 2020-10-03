/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using ERComp.Core.Utilities;
using System.Globalization;

namespace ERComp.PropertyGrid
{
  internal class CommonPropertyExceptionValidationRule : ValidationRule
  {
    private TypeConverter _propertyTypeConverter;
    private Type _type;

    internal CommonPropertyExceptionValidationRule( Type type )
    {
      _propertyTypeConverter = TypeDescriptor.GetConverter( type );
      _type = type;
    }

    public override ValidationResult Validate( object value, CultureInfo cultureInfo )
    {
      ValidationResult result = new ValidationResult( true, null );

      if( GeneralUtilities.CanConvertValue( value, _type ) )
      {
        try
        {
          _propertyTypeConverter.ConvertFrom( value );
        }
        catch( Exception e )
        {
          // Will display a red border in propertyGrid
          result = new ValidationResult( false, e.Message );
        }
      }
      return result;
    }
  }
}
