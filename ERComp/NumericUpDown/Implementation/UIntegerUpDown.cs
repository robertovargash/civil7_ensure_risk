/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp
{
  internal class UIntegerUpDown : CommonNumericUpDown<uint>
  {
    #region Constructors

    static UIntegerUpDown()
    {
      UpdateMetadata( typeof( UIntegerUpDown ), ( uint )1, uint.MinValue, uint.MaxValue );
    }

    public UIntegerUpDown()
      : base( uint.TryParse, Decimal.ToUInt32, ( v1, v2 ) => v1 < v2, ( v1, v2 ) => v1 > v2 )
    {
    }

    #endregion //Constructors

    #region Base Class Overrides

    protected override uint IncrementValue( uint value, uint increment )
    {
      return ( uint )( value + increment );
    }

    protected override uint DecrementValue( uint value, uint increment )
    {
      return ( uint )( value - increment );
    }

    #endregion //Base Class Overrides
  }
}
