﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace ERDock.Layout
{
  [Serializable]
  public abstract class LayoutFloatingWindow : LayoutElement, ILayoutContainer, IXmlSerializable
  {
    #region Constructors

    public LayoutFloatingWindow()
    {
    }

    #endregion

    #region Properties

    #region Children

    public abstract IEnumerable<ILayoutElement> Children
    {
      get;
    }

    #endregion

    #region ChildrenCount

    public abstract int ChildrenCount
    {
      get;
    }

    #endregion

    #region IsValid

    public abstract bool IsValid
    {
      get;
    }

    #endregion

    #endregion

    #region Public Methods

    public abstract void RemoveChild( ILayoutElement element );

    public abstract void ReplaceChild( ILayoutElement oldElement, ILayoutElement newElement );

    public XmlSchema GetSchema()
    {
      return null;
    }

    public abstract void ReadXml( XmlReader reader );

    public virtual void WriteXml( XmlWriter writer )
    {
      foreach( var child in Children )
      {
        var type = child.GetType();
        var serializer = new XmlSerializer( type );
        serializer.Serialize( writer, child );
      }
    }

    #endregion
  }
}
