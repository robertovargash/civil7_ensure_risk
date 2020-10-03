/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using ERComp.Primitives;

namespace ERComp
{
  public class CheckListBox : SelectAllSelector
  {
    #region Constructors

    static CheckListBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( CheckListBox ), new FrameworkPropertyMetadata( typeof( CheckListBox ) ) );
    }

    public CheckListBox()
    {
    }

    #endregion //Constructors

    #region Base Class Override


    #endregion
  }
}
