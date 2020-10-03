/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERComp.PropertyGrid.Attributes
{
  public enum UsageContextEnum
  {
    Alphabetical,
    Categorized,
    Both
  }

  [AttributeUsage( AttributeTargets.Property, AllowMultiple = true, Inherited = true )]
  public class PropertyOrderAttribute : Attribute
  {
    #region Properties

    public int Order
    {
      get;
      set;
    }

    public UsageContextEnum UsageContext
    {
      get;
      set;
    }

    public override object TypeId
    {
      get
      {
        return this;
      }
    }

    #endregion

    #region Initialization

    public PropertyOrderAttribute( int order )
      : this( order, UsageContextEnum.Both )
    {
    }

    public PropertyOrderAttribute( int order, UsageContextEnum usageContext )
    {
      Order = order;
      UsageContext = usageContext;
    }

    #endregion
  }
}
