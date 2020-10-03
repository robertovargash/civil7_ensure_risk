/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERComp.Panels
{
  public class ChildExitedEventArgs : RoutedEventArgs
  {
    #region Constructors

    public ChildExitedEventArgs( UIElement child )
    {
      _child = child;
    }

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
  }
}
