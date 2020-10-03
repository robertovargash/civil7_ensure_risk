/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.Core
{
  public class InvalidContentException : Exception
  {
    #region Constructors

    public InvalidContentException( string message )
      : base( message )
    {
    }

    public InvalidContentException( string message, Exception innerException )
      : base( message, innerException )
    {
    }

    #endregion
  }
}
