/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Layout
{
  [Flags]
  public enum AnchorableShowStrategy : byte
  {
    Most = 0x0001,
    Left = 0x0002,
    Right = 0x0004,
    Top = 0x0010,
    Bottom = 0x0020,
  }
}
