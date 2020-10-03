/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ERComp.Core.Utilities
{
  internal class WeakEventListener<TArgs> : IWeakEventListener where TArgs : EventArgs
  {
    private Action<object,TArgs> _callback;

    public WeakEventListener(Action<object,TArgs> callback)
    {
      if( callback == null )
         throw new ArgumentNullException( "callback" );

      _callback = callback;
    }

    public bool ReceiveWeakEvent( Type managerType, object sender, EventArgs e )
    {
      _callback(sender, (TArgs)e);
      return true;
    }
  }
}
