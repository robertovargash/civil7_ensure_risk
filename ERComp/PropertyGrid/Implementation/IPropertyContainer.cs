/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/


using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.Windows;
namespace ERComp.PropertyGrid
{
  internal interface IPropertyContainer
  {







    ContainerHelperBase ContainerHelper { get; }

    Style PropertyContainerStyle { get; }

    EditorDefinitionCollection EditorDefinitions { get; }

    PropertyDefinitionCollection PropertyDefinitions { get; }

    bool IsCategorized { get; }

    bool IsSortedAlphabetically { get; }

    bool AutoGenerateProperties { get; }

    bool HideInheritedProperties { get; }

    FilterInfo FilterInfo { get; }

    bool? IsPropertyVisible( PropertyDescriptor pd );

  }
}
