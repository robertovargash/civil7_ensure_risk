/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

namespace ERComp
{
  public interface IRichTextBoxFormatBar
  {
    /// <summary>
    /// Represents the RichTextBox that will be the target for all text manipulations in the format bar.
    /// </summary>
    System.Windows.Controls.RichTextBox Target
    {
      get;
      set;
    }

    /// <summary>
    /// Represents the property that will be used to know if the formatBar should fade when mouse goes away.
    /// </summary>
    bool PreventDisplayFadeOut
    {
      get;
    }

    /// <summary>
    /// Represents the Method that will be used to update the format bar values based on the Selection.
    /// </summary>
    void Update();
  }
}
