﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Xml.Serialization;
using System.IO;

namespace ERDock.Layout.Serialization
{
  public class XmlLayoutSerializer : LayoutSerializer
  {
    #region Constructors

    public XmlLayoutSerializer( DockingManager manager )
        : base( manager )
    {
    }

    #endregion

    #region Public Methods

    public void Serialize( System.Xml.XmlWriter writer )
    {
      var serializer = new XmlSerializer( typeof( LayoutRoot ) );
      serializer.Serialize( writer, Manager.Layout );
    }

    public void Serialize( System.IO.TextWriter writer )
    {
      var serializer = new XmlSerializer( typeof( LayoutRoot ) );
      serializer.Serialize( writer, Manager.Layout );
    }

    public void Serialize( System.IO.Stream stream )
    {
      var serializer = new XmlSerializer( typeof( LayoutRoot ) );
      serializer.Serialize( stream, Manager.Layout );
    }

    public void Serialize( string filepath )
    {
      using( var stream = new StreamWriter( filepath ) )
        Serialize( stream );
    }

    public void Deserialize( System.IO.Stream stream )
    {
      try
      {
        StartDeserialization();
        var serializer = new XmlSerializer( typeof( LayoutRoot ) );
        var layout = serializer.Deserialize( stream ) as LayoutRoot;
        FixupLayout( layout );
        Manager.Layout = layout;
      }
      finally
      {
        EndDeserialization();
      }
    }

    public void Deserialize( System.IO.TextReader reader )
    {
      try
      {
        StartDeserialization();
        var serializer = new XmlSerializer( typeof( LayoutRoot ) );
        var layout = serializer.Deserialize( reader ) as LayoutRoot;
        FixupLayout( layout );
        Manager.Layout = layout;
      }
      finally
      {
        EndDeserialization();
      }
    }

    public void Deserialize( System.Xml.XmlReader reader )
    {
      try
      {
        StartDeserialization();
        var serializer = new XmlSerializer( typeof( LayoutRoot ) );
        var layout = serializer.Deserialize( reader ) as LayoutRoot;
        FixupLayout( layout );
        Manager.Layout = layout;
      }
      finally
      {
        EndDeserialization();
      }
    }

    public void Deserialize( string filepath )
    {
      using( var stream = new StreamReader( filepath ) )
        Deserialize( stream );
    }

    #endregion
  }
}
