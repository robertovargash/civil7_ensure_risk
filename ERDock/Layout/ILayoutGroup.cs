﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Layout
{
  public interface ILayoutGroup : ILayoutContainer
  {
    int IndexOfChild( ILayoutElement element );
    void InsertChildAt( int index, ILayoutElement element );
    void RemoveChildAt( int index );
    void ReplaceChildAt( int index, ILayoutElement element );

    event EventHandler ChildrenCollectionChanged;
  }
}
