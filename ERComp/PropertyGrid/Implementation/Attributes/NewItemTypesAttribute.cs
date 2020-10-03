/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;

namespace ERComp.PropertyGrid.Attributes
{
  /// <summary>
  /// This attribute can decorate the collection properties (i.e., IList) 
  /// of your selected object in order to control the types that will be allowed
  /// to be instantiated in the CollectionControl.
  /// </summary>
  [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
  public class NewItemTypesAttribute : Attribute
  {
    public IList<Type> Types
    {
      get;
      set;
    }

    public NewItemTypesAttribute( params Type[] types )
    {
      this.Types = new List<Type>( types );
    }

    public NewItemTypesAttribute()
    {
    }
  }
}
