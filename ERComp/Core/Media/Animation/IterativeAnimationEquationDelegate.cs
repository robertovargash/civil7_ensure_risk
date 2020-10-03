/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.Media.Animation
{
  public delegate T IterativeAnimationEquationDelegate<T>( TimeSpan currentTime, T from, T to, TimeSpan duration );
}
