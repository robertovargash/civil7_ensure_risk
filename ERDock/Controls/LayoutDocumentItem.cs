﻿/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using ERDock.Layout;
using System.Windows;
using ERDock.Commands;
using System.Windows.Input;
using System.Windows.Data;

namespace ERDock.Controls
{
  public class LayoutDocumentItem : LayoutItem
  {
    #region Members

    private LayoutDocument _document;

    #endregion

    #region Constructors

    internal LayoutDocumentItem()
    {
    }

    #endregion

    #region Properties

    #region Description

    /// <summary>
    /// Description Dependency Property
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register( "Description", typeof( string ), typeof( LayoutDocumentItem ),
                new FrameworkPropertyMetadata( ( string )null, new PropertyChangedCallback( OnDescriptionChanged ) ) );

    /// <summary>
    /// Gets or sets the Description property.  This dependency property 
    /// indicates the description to display for the document item.
    /// </summary>
    public string Description
    {
      get
      {
        return ( string )GetValue( DescriptionProperty );
      }
      set
      {
        SetValue( DescriptionProperty, value );
      }
    }

    /// <summary>
    /// Handles changes to the Description property.
    /// </summary>
    private static void OnDescriptionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ( ( LayoutDocumentItem )d ).OnDescriptionChanged( e );
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the Description property.
    /// </summary>
    protected virtual void OnDescriptionChanged( DependencyPropertyChangedEventArgs e )
    {
      _document.Description = ( string )e.NewValue;
    }

    #endregion

    #endregion

    #region Overrides

    protected override void Close()
    {
      if( ( _document.Root != null ) && ( _document.Root.Manager != null ) )
      {
        var dockingManager = _document.Root.Manager;
        dockingManager._ExecuteCloseCommand( _document );
      }
    }

    protected override void OnVisibilityChanged()
    {
      if( (_document != null) && (_document.Root != null) )
      {
        _document.IsVisible = ( this.Visibility == Visibility.Visible );

        if( _document.Parent is LayoutDocumentPane )
        {
          ( ( LayoutDocumentPane )_document.Parent ).ComputeVisibility();
        }
      }

      base.OnVisibilityChanged();
    }







    internal override void Attach( LayoutContent model )
    {
      _document = model as LayoutDocument;
      base.Attach( model );
    }

    internal override void Detach()
    {
      _document = null;
      base.Detach();
    }

    #endregion

    #region Private Methods







#endregion
  }
}
