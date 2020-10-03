/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp
{
  public class TimeItem
  {
    public string Display
    {
      get;
      set;
    }
    public TimeSpan Time
    {
      get;
      set;
    }

    public TimeItem( string display, TimeSpan time )
    {
      Display = display;
      Time = time;
    }

    #region Base Class Overrides

    public override bool Equals( object obj )
    {
      var item = obj as TimeItem;
      if( item != null )
        return Time == item.Time;
      else
        return false;
    }

    public override int GetHashCode()
    {
      return Time.GetHashCode();
    }

    #endregion //Base Class Overrides
  }
}
