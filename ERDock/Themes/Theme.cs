/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;
using System.Windows;

namespace ERDock.Themes
{
  public abstract class Theme : DependencyObject
  {
    public Theme()
    {
    }

    public abstract Uri GetResourceUri();
  }
}
