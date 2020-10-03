/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Input;

namespace ERComp.PropertyGrid.Commands
{
  public class PropertyGridCommands
  {
    private static RoutedCommand _clearFilterCommand = new RoutedCommand();
    public static RoutedCommand ClearFilter
    {
      get
      {
        return _clearFilterCommand;
      }
    }
  }
}
