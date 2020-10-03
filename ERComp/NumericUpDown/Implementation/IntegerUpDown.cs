/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp
{
  public class IntegerUpDown : CommonNumericUpDown<int>
  {
    #region Constructors

    static IntegerUpDown()
    {
      UpdateMetadata( typeof( IntegerUpDown ), 1, int.MinValue, int.MaxValue );
    }

    public IntegerUpDown()
      : base( Int32.TryParse, Decimal.ToInt32, ( v1, v2 ) => v1 < v2, ( v1, v2 ) => v1 > v2 )
    {
    }

    #endregion //Constructors

    #region Base Class Overrides

    protected override int IncrementValue( int value, int increment )
    {
      return value + increment;
    }

    protected override int DecrementValue( int value, int increment )
    {
      return value - increment;
    }

    #endregion //Base Class Overrides
  }
}
