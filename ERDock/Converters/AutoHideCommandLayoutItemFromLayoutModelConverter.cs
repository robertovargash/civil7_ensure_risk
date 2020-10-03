/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows.Data;
using ERDock.Layout;
using ERDock.Controls;

namespace ERDock.Converters
{
  public class AutoHideCommandLayoutItemFromLayoutModelConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      //when this converter is called layout could be constructing so many properties here are potentially not valid
      var layoutModel = value as LayoutContent;
      if( layoutModel == null )
        return null;
      if( layoutModel.Root == null )
        return null;
      if( layoutModel.Root.Manager == null )
        return null;

      var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel( layoutModel ) as LayoutAnchorableItem;
      if( layoutItemModel == null )
        return Binding.DoNothing;

      return layoutItemModel.AutoHideCommand;
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
