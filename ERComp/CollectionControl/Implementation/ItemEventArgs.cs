/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ERComp
{
  public class ItemEventArgs : RoutedEventArgs
  {
    #region Protected Members

    private object _item;

    #endregion

    #region Constructor

    internal ItemEventArgs( RoutedEvent routedEvent, object newItem )
      : base( routedEvent )
    {
      _item = newItem;
    }

    #endregion

    #region Property Item

    public object Item
    {
      get
      {
        return _item;
      }
    }

    #endregion
  }
}
