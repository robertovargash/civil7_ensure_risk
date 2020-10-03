/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

#region Using directives

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Markup;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "CIVIL 7 COMPONENTS" )]
[assembly: AssemblyDescription("This assembly implements various Windows Presentation Framework controls.")]

[assembly: AssemblyCompany("CIVIL 7 Co.")]
[assembly: AssemblyProduct( "ENSURE RISK PROJECT for WPF" )]
[assembly: AssemblyCopyright( "Copyright (C) CIVIL 7 NL 2020" )]
[assembly: AssemblyCulture( "" )]


// Needed to enable xbap scenarios
[assembly: AllowPartiallyTrustedCallers]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]


[assembly: InternalsVisibleTo( "ERComp.Themes.Office2007" + ",PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100d59d8147eb2015" +
    "ca98a92da860fd766d101271d8c2f545894870fd6183255737d79347bbf5250291ae75651e1150" +
    "1b7452ee003b80b936614cdda51db8eb6f8fde913e67d45395b480a992be17bf04744a7fe803ea" +
    "131b925dcf84a73d22264352eca7c3fcf9387f3eee1d60ac7974f04866e6c72928dc0609abe341" +
    "f92cbfb5")]

[assembly: InternalsVisibleTo( "ERComp.Themes.Metro" + ",PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100d59d8147eb2015" +
    "ca98a92da860fd766d101271d8c2f545894870fd6183255737d79347bbf5250291ae75651e1150" +
    "1b7452ee003b80b936614cdda51db8eb6f8fde913e67d45395b480a992be17bf04744a7fe803ea" +
    "131b925dcf84a73d22264352eca7c3fcf9387f3eee1d60ac7974f04866e6c72928dc0609abe341" +
    "f92cbfb5")]

[assembly: InternalsVisibleTo( "ERComp.Themes.Windows10" + ",PublicKey=" +
    "0024000004800000940000000602000000240000525341310004000001000100d59d8147eb2015" +
    "ca98a92da860fd766d101271d8c2f545894870fd6183255737d79347bbf5250291ae75651e1150" +
    "1b7452ee003b80b936614cdda51db8eb6f8fde913e67d45395b480a992be17bf04744a7fe803ea" +
    "131b925dcf84a73d22264352eca7c3fcf9387f3eee1d60ac7974f04866e6c72928dc0609abe341" +
    "f92cbfb5")]





//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://civil7.nl/wpf/xaml/toolkit", "xctk")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Core.Converters" )]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Core.Input" )]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Core.Media")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Core.Utilities")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Chromes")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Primitives")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.PropertyGrid")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.PropertyGrid.Attributes")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.PropertyGrid.Commands")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.PropertyGrid.Converters")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.PropertyGrid.Editors")]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Zoombox" )]
[assembly: XmlnsDefinition("http://civil7.nl/wpf/xaml/toolkit", "ERComp.Panels" )]


#pragma warning disable 1699
[assembly: AssemblyDelaySign( false )]
//[assembly: AssemblyKeyFile( @"..\..\sn.snk" )]
[assembly: AssemblyKeyName( "" )]
#pragma warning restore 1699


