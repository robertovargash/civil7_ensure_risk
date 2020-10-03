/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Layout
{
  public enum ChildrenTreeChange
  {
    /// <summary>
    /// Direct insert/remove operation has been perfomed to the group
    /// </summary>
    DirectChildrenChanged,

    /// <summary>
    /// An element below in the hierarchy as been added/removed
    /// </summary>
    TreeChanged
  }

  public class ChildrenTreeChangedEventArgs : EventArgs
  {
    public ChildrenTreeChangedEventArgs( ChildrenTreeChange change )
    {
      Change = change;
    }

    public ChildrenTreeChange Change
    {
      get; private set;
    }
  }
}
