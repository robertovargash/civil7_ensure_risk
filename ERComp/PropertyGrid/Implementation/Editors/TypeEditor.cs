﻿/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using System.Windows.Data;
using ERComp.Primitives;

namespace ERComp.PropertyGrid.Editors
{
  public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
  {
    #region Properties

    protected T Editor
    {
      get;
      set;
    }
    protected DependencyProperty ValueProperty
    {
      get;
      set;
    }

    #endregion //Properties

    #region ITypeEditor Members

    public virtual FrameworkElement ResolveEditor( PropertyItem propertyItem )
    {
      Editor = this.CreateEditor();
      SetValueDependencyProperty();
      SetControlProperties( propertyItem );
      ResolveValueBinding( propertyItem );
      return Editor;
    }

    #endregion //ITypeEditor Members

    #region Methods

    protected virtual T CreateEditor()
    {
      return new T();
    }

    protected virtual IValueConverter CreateValueConverter()
    {
      return null;
    }

    protected virtual void ResolveValueBinding( PropertyItem propertyItem )
    {
      var _binding = new Binding( "Value" );
      _binding.Source = propertyItem;
      _binding.UpdateSourceTrigger = (Editor is InputBase) ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.Default;
      _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
      _binding.Converter = CreateValueConverter();
      BindingOperations.SetBinding( Editor, ValueProperty, _binding );
    }

    protected virtual void SetControlProperties( PropertyItem propertyItem )
    {
      //TODO: implement in derived class
    }

    protected abstract void SetValueDependencyProperty();

    #endregion //Methods
  }
}
