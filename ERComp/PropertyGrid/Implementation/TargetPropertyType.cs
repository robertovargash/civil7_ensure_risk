﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERComp.Core.Utilities;

namespace ERComp.PropertyGrid
{
  /// <summary>
  /// This class is intended to provide the "Type" target
  /// for property definitions or editor definitions when
  /// using Property Element Syntax.
  /// </summary>
  public sealed class TargetPropertyType
  {
    private Type _type;
    private bool _sealed;

    public Type Type
    {
      get { return _type; }
      set 
      {
        if( _sealed )
          throw new InvalidOperationException(
            string.Format(
            "{0}.Type property cannot be modified once the instance is used",
            typeof( TargetPropertyType ) ) );

        _type = value; 
      }
    }

    internal void Seal()
    {
      if( _type == null )
        throw new InvalidOperationException(
          string.Format( "{0}.Type property must be initialized", typeof( TargetPropertyType ) ) );

      _sealed = true;
    }
  }
}
