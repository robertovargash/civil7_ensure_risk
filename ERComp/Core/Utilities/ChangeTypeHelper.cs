﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ERComp.Core.Utilities
{
  internal static class ChangeTypeHelper
  {
    internal static object ChangeType( object value, Type conversionType, IFormatProvider provider )
    {
      if( conversionType == null )
      {
        throw new ArgumentNullException( "conversionType" );
      }
      if( conversionType == typeof( Guid ) )
      {
        return new Guid( value.ToString() );
      }
      else if( conversionType == typeof( Guid? ) )
      {
        if( value == null )
          return null;
        return new Guid( value.ToString() );
      }
      else if( conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) )
      {
        if( value == null )
          return null;
        NullableConverter nullableConverter = new NullableConverter( conversionType );
        conversionType = nullableConverter.UnderlyingType;
      }

      return System.Convert.ChangeType( value, conversionType, provider );
    }
  }
}
