/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;
using System.Windows.Data;

namespace ERDock.Controls
{
  public class ContextMenuEx : ContextMenu
  {
    #region Constructors

    static ContextMenuEx()
    {
    }

    public ContextMenuEx()
    {
    }

    #endregion

    #region Overrides

    protected override System.Windows.DependencyObject GetContainerForItemOverride()
    {
      return new MenuItemEx();
    }

    protected override void OnOpened( System.Windows.RoutedEventArgs e )
    {
      BindingOperations.GetBindingExpression( this, ItemsSourceProperty ).UpdateTarget();

      base.OnOpened( e );
    }

    #endregion
  }
}
