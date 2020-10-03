/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System;

namespace ERDock.Themes
{
  public class GenericTheme : Theme
  {
    public override Uri GetResourceUri()
    {
      return new Uri( "/ERDock;component/Themes/generic.xaml", UriKind.Relative );
    }
  }
}
