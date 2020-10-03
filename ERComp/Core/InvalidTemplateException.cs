/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.Core
{
  public class InvalidTemplateException : Exception
  {
    #region Constructors

    public InvalidTemplateException( string message )
      : base( message )
    {
    }

    public InvalidTemplateException( string message, Exception innerException )
      : base( message, innerException )
    {
    }

    #endregion
  }
}
