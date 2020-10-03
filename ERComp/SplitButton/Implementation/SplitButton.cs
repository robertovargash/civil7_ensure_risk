/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;
using System.Windows.Controls;

namespace ERComp
{
  [TemplatePart( Name = PART_ActionButton, Type = typeof( Button ) )]
  public class SplitButton : DropDownButton
  {
    private const string PART_ActionButton = "PART_ActionButton";

    #region Constructors

    static SplitButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( SplitButton ), new FrameworkPropertyMetadata( typeof( SplitButton ) ) );
    }

    #endregion //Constructors

    #region Base Class Overrides

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      Button = GetTemplateChild( PART_ActionButton ) as Button;
    }


  #endregion //Base Class Overrides
  }
}
