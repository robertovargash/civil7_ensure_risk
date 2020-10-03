/*************************************************************************************

   

   

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows.Media;

namespace ERComp
{
  public class ColorItem
  {
    public Color? Color
    {
      get;
      set;
    }
    public string Name
    {
      get;
      set;
    }

    public ColorItem( Color? color, string name )
    {
      Color = color;
      Name = name;
    }

    public override bool Equals(object obj)
    {
      var ci = obj as ColorItem;
      if (ci == null)
          return false;
      return ( ci.Color.Equals( Color ) && ci.Name.Equals( Name ) );
    }

    public override int GetHashCode()
    {
      return this.Color.GetHashCode() ^ this.Name.GetHashCode();
    }
  }
}
