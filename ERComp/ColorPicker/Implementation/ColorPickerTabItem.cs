/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Controls;
using System.Windows.Input;

namespace ERComp
{
  public class ColorPickerTabItem : TabItem
  {
    protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
    {
      if( e.Source == this || !this.IsSelected )
        return;

      base.OnMouseLeftButtonDown( e );
    }

    protected override void OnMouseLeftButtonUp( MouseButtonEventArgs e )
    {
      //Selection on Mouse Up
      if( e.Source == this || !this.IsSelected )
      {
        base.OnMouseLeftButtonDown( e );
      }

      base.OnMouseLeftButtonUp( e );
    }
  }
}
