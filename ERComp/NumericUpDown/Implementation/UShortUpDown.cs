/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp
{
  internal class UShortUpDown : CommonNumericUpDown<ushort>
  {
    #region Constructors

    static UShortUpDown()
    {
      UpdateMetadata( typeof( UShortUpDown ), ( ushort )1, ushort.MinValue, ushort.MaxValue );
    }

    public UShortUpDown()
      : base( ushort.TryParse, Decimal.ToUInt16, ( v1, v2 ) => v1 < v2, ( v1, v2 ) => v1 > v2 )
    {
    }

    #endregion //Constructors

    #region Base Class Overrides

    protected override ushort IncrementValue( ushort value, ushort increment )
    {
      return ( ushort )( value + increment );
    }

    protected override ushort DecrementValue( ushort value, ushort increment )
    {
      return ( ushort )( value - increment );
    }

    #endregion //Base Class Overrides
  }
}
