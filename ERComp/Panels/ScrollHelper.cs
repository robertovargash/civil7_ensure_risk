﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;
using ERComp.Core.Utilities;

namespace ERComp.Panels
{
  internal static class ScrollHelper
  {
    public static bool ScrollLeastAmount( Rect physViewRect, Rect itemRect, out Vector newPhysOffset )
    {
      bool scrollNeeded = false;

      newPhysOffset = new Vector();

      if( physViewRect.Contains( itemRect ) == false )
      {
        // Check if child is inside the view horizontially.
        if( itemRect.Left > physViewRect.Left && itemRect.Right < physViewRect.Right ||
            DoubleHelper.AreVirtuallyEqual( itemRect.Left, physViewRect.Left ) == true )
        {
          newPhysOffset.X = itemRect.Left;
        }
        // Child is to the left of the view or is it bigger than the view
        else if( itemRect.Left < physViewRect.Left || itemRect.Width > physViewRect.Width )
        {
          newPhysOffset.X = itemRect.Left;
        }
        // Child is to the right of the view
        else
        {
          newPhysOffset.X = Math.Max( 0, physViewRect.Left + ( itemRect.Right - physViewRect.Right ) );
        }

        // Check if child is inside the view vertically.
        if( itemRect.Top > physViewRect.Top && itemRect.Bottom < physViewRect.Bottom ||
            DoubleHelper.AreVirtuallyEqual( itemRect.Top, physViewRect.Top ) == true )
        {
          newPhysOffset.Y = itemRect.Top;
        }
        // Child is the above the view or is it bigger than the view
        else if( itemRect.Top < physViewRect.Top || itemRect.Height > physViewRect.Height )
        {
          newPhysOffset.Y = itemRect.Top;
        }
        // Child is below the view
        else
        {
          newPhysOffset.Y = Math.Max( 0, physViewRect.Top + ( itemRect.Bottom - physViewRect.Bottom ) );
        }

        scrollNeeded = true;
      }

      return scrollNeeded;
    }
  }
}
