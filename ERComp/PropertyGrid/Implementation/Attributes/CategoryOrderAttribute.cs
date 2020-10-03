/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.PropertyGrid.Attributes
{
  [AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
  public class CategoryOrderAttribute : Attribute
  {
    #region Properties

    #region Order

    public int Order
    {
      get;
      set;
    }

    #endregion

    #region Category

    public virtual string Category
    {
      get
      {
        return CategoryValue;
      }
    }

    #endregion

    #region CategoryValue

    public string CategoryValue
    {
      get;
      private set;
    }

    #endregion

    #endregion

    #region constructor

    public CategoryOrderAttribute()
    {
    }

    public CategoryOrderAttribute( string categoryName, int order )
      :this()
    {
      CategoryValue = categoryName;
      Order = order;
    }

    #endregion
  }
}

