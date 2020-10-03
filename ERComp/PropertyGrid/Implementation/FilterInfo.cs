/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERComp.PropertyGrid
{
  internal struct FilterInfo
  {
    public string InputString;
    public Predicate<object> Predicate;
  }
}
