﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp.Core
{
  public class PropertyChangedEventArgs<T> : RoutedEventArgs
  {
    #region Constructors

    public PropertyChangedEventArgs( RoutedEvent Event, T oldValue, T newValue )
      : base()
    {
      _oldValue = oldValue;
      _newValue = newValue;
      this.RoutedEvent = Event;
    }

    #endregion

    #region NewValue Property

    public T NewValue
    {
      get
      {
        return _newValue;
      }
    }

    private readonly T _newValue;

    #endregion

    #region OldValue Property

    public T OldValue
    {
      get
      {
        return _oldValue;
      }
    }

    private readonly T _oldValue;

    #endregion

    protected override void InvokeEventHandler( Delegate genericHandler, object genericTarget )
    {
      PropertyChangedEventHandler<T> handler = ( PropertyChangedEventHandler<T> )genericHandler;
      handler( genericTarget, this );
    }
  }
}
