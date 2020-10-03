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
  public class ItemAddingEventArgs : CancelRoutedEventArgs
  {
    #region Constructor

    public ItemAddingEventArgs( RoutedEvent itemAddingEvent, object itemAdding )
      : base( itemAddingEvent )
    {
      Item = itemAdding;
    }

    #endregion

    #region Properties

    #region Item Property

    public object Item
    {
      get;
      set;
    }

    #endregion

    #endregion //Properties
  }
}
