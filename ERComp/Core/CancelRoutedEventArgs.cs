﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ERComp.Core
{
  public delegate void CancelRoutedEventHandler( object sender, CancelRoutedEventArgs e );

  /// <summary>
  /// An event data class that allows to inform the sender that the handler wants to cancel
  /// the ongoing action.
  /// 
  /// The handler can set the "Cancel" property to false to cancel the action.
  /// </summary>
  public class CancelRoutedEventArgs : RoutedEventArgs
  {
    public CancelRoutedEventArgs()
      : base()
    {
    }

    public CancelRoutedEventArgs( RoutedEvent routedEvent )
      : base( routedEvent )
    {
    }

    public CancelRoutedEventArgs( RoutedEvent routedEvent, object source )
      : base( routedEvent, source )
    {
    }

    #region Cancel Property

    public bool Cancel
    {
      get;
      set;
    }

    #endregion Cancel Property
  }
}
