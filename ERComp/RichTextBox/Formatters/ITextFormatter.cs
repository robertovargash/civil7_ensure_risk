/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Documents;

namespace ERComp
{
  public interface ITextFormatter
  {
    string GetText( FlowDocument document );
    void SetText( FlowDocument document, string text );
  }
}
