/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERComp.Core;
using System.Windows;

namespace ERComp
{
  public class ItemDeletingEventArgs : CancelRoutedEventArgs
  {
    #region Private Members

    private object _item;

    #endregion

    #region Constructor

    public ItemDeletingEventArgs( RoutedEvent itemDeletingEvent, object itemDeleting )
      : base( itemDeletingEvent )
    {
      _item = itemDeleting;
    }

    #region Property Item

    public object Item
    {
      get
      {
        return _item;
      }
    }

    #endregion

    #endregion
  }
}
