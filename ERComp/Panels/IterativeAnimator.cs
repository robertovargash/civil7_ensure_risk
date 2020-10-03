/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.ComponentModel;
using System.Windows;
using ERComp.Media.Animation;
using ERComp.Core;

namespace ERComp.Panels
{
  [TypeConverter( typeof( AnimatorConverter ) )]
  public abstract class IterativeAnimator
  {
    #region Default Static Property

    public static IterativeAnimator Default
    {
      get
      {
        return _default;
      }
    }

    private static readonly IterativeAnimator _default = new DefaultAnimator();

    #endregion

    public abstract Rect GetInitialChildPlacement(
      UIElement child,
      Rect currentPlacement,
      Rect targetPlacement,
      AnimationPanel activeLayout,
      ref AnimationRate animationRate,
      out object placementArgs,
      out bool isDone );

    public abstract Rect GetNextChildPlacement(
      UIElement child,
      TimeSpan currentTime,
      Rect currentPlacement,
      Rect targetPlacement,
      AnimationPanel activeLayout,
      AnimationRate animationRate,
      ref object placementArgs,
      out bool isDone );

    #region DefaultAnimator Nested Type

    private sealed class DefaultAnimator : IterativeAnimator
    {
      public override Rect GetInitialChildPlacement( UIElement child, Rect currentPlacement, Rect targetPlacement, AnimationPanel activeLayout, ref AnimationRate animationRate, out object placementArgs, out bool isDone )
      {
        throw new InvalidOperationException( ErrorMessages.GetMessage( ErrorMessages.DefaultAnimatorCantAnimate ) );
      }

      public override Rect GetNextChildPlacement( UIElement child, TimeSpan currentTime, Rect currentPlacement, Rect targetPlacement, AnimationPanel activeLayout, AnimationRate animationRate, ref object placementArgs, out bool isDone )
      {
        throw new InvalidOperationException( ErrorMessages.GetMessage( ErrorMessages.DefaultAnimatorCantAnimate ) );
      }
    }

    #endregion
  }
}
