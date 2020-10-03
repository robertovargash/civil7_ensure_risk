/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERComp.PropertyGrid.Editors
{
  public interface ITypeEditor
  {
    FrameworkElement ResolveEditor( PropertyItem propertyItem );
  }
}
