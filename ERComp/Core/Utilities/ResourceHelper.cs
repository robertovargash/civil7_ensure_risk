/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.IO;
using System.Reflection;
using System.Resources;

namespace ERComp.Core.Utilities
{
  internal class ResourceHelper
  {
    internal static Stream LoadResourceStream( Assembly assembly, string resId )
    {
      string basename = System.IO.Path.GetFileNameWithoutExtension( assembly.ManifestModule.Name ) + ".g";
      ResourceManager resourceManager = new ResourceManager( basename, assembly );

      // resource names are lower case and contain only forward slashes
      resId = resId.ToLower();
      resId = resId.Replace( '\\', '/' );
      return ( resourceManager.GetObject( resId ) as Stream );
    }
  }
}
