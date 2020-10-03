/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.PropertyGrid.Attributes
{
  public class ItemsSourceAttribute : Attribute
  {
    public Type Type
    {
      get;
      set;
    }

    public ItemsSourceAttribute( Type type )
    {
      var valueSourceInterface = type.GetInterface( typeof( IItemsSource ).FullName );
      if( valueSourceInterface == null )
        throw new ArgumentException( "Type must implement the IItemsSource interface.", "type" );

      Type = type;
    }
  }
}
