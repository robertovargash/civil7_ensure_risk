/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERComp.Core
{
  public class IndexChangedEventArgs : PropertyChangedEventArgs<int>
  {
    #region Constructors

    public IndexChangedEventArgs( RoutedEvent routedEvent, int oldIndex, int newIndex )
      : base( routedEvent, oldIndex, newIndex )
    {
    }

    #endregion

    protected override void InvokeEventHandler( Delegate genericHandler, object genericTarget )
    {
      ( ( IndexChangedEventHandler )genericHandler )( genericTarget, this );
    }
  }
}
