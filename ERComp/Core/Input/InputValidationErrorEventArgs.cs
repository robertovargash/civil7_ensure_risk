﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERComp.Core.Input
{
  public delegate void InputValidationErrorEventHandler( object sender, InputValidationErrorEventArgs e );

  public class InputValidationErrorEventArgs : EventArgs
  {
    #region Constructors

    public InputValidationErrorEventArgs( Exception e )
    {
      Exception = e;
    }

    #endregion

    #region Exception Property

    public Exception Exception
    {
      get
      {
        return exception;
      }
      private set
      {
        exception = value;
      }
    }

    private Exception exception;

    #endregion

    #region ThrowException Property

    public bool ThrowException
    {
      get
      {
        return _throwException;
      }
      set
      {
        _throwException = value;
      }
    }

    private bool _throwException;

    #endregion
  }
}
