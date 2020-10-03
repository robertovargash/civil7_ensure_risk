﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp.Core.Utilities
{
  internal static class PointHelper
  {
    public static double DistanceBetween( Point p1, Point p2 )
    {
      return Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) );
    }

    public static Point Empty
    {
      get
      {
        return new Point( double.NaN, double.NaN );
      }
    }

    public static bool IsEmpty( Point point )
    {
      return DoubleHelper.IsNaN( point.X ) && DoubleHelper.IsNaN( point.Y );
    }
  }
}
