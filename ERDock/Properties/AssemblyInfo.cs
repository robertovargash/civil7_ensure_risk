/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using System;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "ERDock" )]
[assembly: AssemblyDescription( "This assembly implements the ERDock namespace, a docking layout system for the Windows Presentation Framework." )]

[assembly: AssemblyCompany( "CIVIL 7 NL" )]
[assembly: AssemblyProduct( "Ensure Risk Project" )]
[assembly: AssemblyCopyright( "Copyright (C) CIVIL7 Co. 2020" )]
[assembly: AssemblyCulture( "" )]



// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant( true )]


//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix( "http://civil7.nl/wpf/xaml/avalondock", "xcad" )]
[assembly: XmlnsDefinition( "http://civil7.nl/wpf/xaml/avalondock", "ERDock" )]
[assembly: XmlnsDefinition( "http://civil7.nl/wpf/xaml/avalondock", "ERDock.Controls" )]
[assembly: XmlnsDefinition( "http://civil7.nl/wpf/xaml/avalondock", "ERDock.Converters" )]
[assembly: XmlnsDefinition( "http://civil7.nl/wpf/xaml/avalondock", "ERDock.Layout" )]
[assembly: XmlnsDefinition( "http://civil7.nl/wpf/xaml/avalondock", "ERDock.Themes" )]

#pragma warning disable 1699
[assembly: AssemblyDelaySign( false )]
//[assembly: AssemblyKeyFile( @"..\..\sn.snk" )]
[assembly: AssemblyKeyName( "" )]
#pragma warning restore 1699


