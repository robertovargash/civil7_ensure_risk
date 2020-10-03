/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp
{
  [Flags]
  public enum AllowedSpecialValues
  {
    None = 0,
    NaN = 1,
    PositiveInfinity = 2,
    NegativeInfinity = 4,
    AnyInfinity = PositiveInfinity | NegativeInfinity,
    Any = NaN | AnyInfinity
  }
}
