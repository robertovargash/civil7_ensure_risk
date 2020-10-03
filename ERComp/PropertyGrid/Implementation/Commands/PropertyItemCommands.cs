/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Input;

namespace ERComp.PropertyGrid.Commands
{
  public static class PropertyItemCommands
  {
    private static RoutedCommand _resetValueCommand = new RoutedCommand();
    public static RoutedCommand ResetValue
    {
      get
      {
        return _resetValueCommand;
      }
    }
  }
}
