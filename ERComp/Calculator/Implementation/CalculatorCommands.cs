using System.Windows.Input;

namespace ERComp
{
  public static class CalculatorCommands
  {
    private static RoutedCommand _calculatorButtonClickCommand = new RoutedCommand();

    public static RoutedCommand CalculatorButtonClick
    {
      get
      {
        return _calculatorButtonClickCommand;
      }
    }
  }
}
