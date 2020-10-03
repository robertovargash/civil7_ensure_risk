/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERComp.Core.Input
{
  public interface IValidateInput
  {
    event InputValidationErrorEventHandler InputValidationError;
    bool CommitInput();
  }
}
