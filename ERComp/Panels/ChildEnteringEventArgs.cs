﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERComp.Panels
{
  public class ChildEnteringEventArgs : RoutedEventArgs
  {
    #region Constructors

    public ChildEnteringEventArgs( UIElement child, Rect? enterFrom, Rect arrangeRect )
    {
      _child = child;
      _enterFrom = enterFrom;
      _arrangeRect = arrangeRect;
    }

    #endregion

    #region ArrangeRect Property

    public Rect ArrangeRect
    {
      get
      {
        return _arrangeRect;
      }
    }

    private readonly Rect _arrangeRect;

    #endregion

    #region Child Property

    public UIElement Child
    {
      get
      {
        return _child;
      }
    }

    private readonly UIElement _child;

    #endregion

    #region EnterFrom Property

    public Rect? EnterFrom
    {
      get
      {
        return _enterFrom;
      }
      set
      {
        _enterFrom = value;
      }
    }

    private Rect? _enterFrom; //null

    #endregion
  }
}
