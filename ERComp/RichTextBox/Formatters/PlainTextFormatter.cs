/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Documents;

namespace ERComp
{
  /// <summary>
  /// Formats the RichTextBox text as plain text
  /// </summary>
  public class PlainTextFormatter : ITextFormatter
  {
    public string GetText( FlowDocument document )
    {
      return new TextRange( document.ContentStart, document.ContentEnd ).Text;
    }

    public void SetText( FlowDocument document, string text )
    {
      new TextRange( document.ContentStart, document.ContentEnd ).Text = text;
    }
  }
}
