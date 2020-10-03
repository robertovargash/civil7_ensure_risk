/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERDock.Themes
{
  public abstract class DictionaryTheme : Theme
  {
    #region Constructors

    public DictionaryTheme()
    {
    }

    public DictionaryTheme( ResourceDictionary themeResourceDictionary )
    {
      this.ThemeResourceDictionary = themeResourceDictionary;
    }

    #endregion

    #region Properties

    public ResourceDictionary ThemeResourceDictionary
    {
      get;
      private set;
    }

    #endregion

    #region Overrides

    public override Uri GetResourceUri()
    {
      return null;
    }

    #endregion
  }
}
