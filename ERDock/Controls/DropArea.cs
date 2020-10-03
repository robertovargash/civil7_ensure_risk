/*************************************************************************************
   
 

 

   This program is provided to you under the terms of the Microsoft Public
    

   For more features, controls, and fast professional support,
    

    

  ***********************************************************************************/

using System.Windows;

namespace ERDock.Controls
{
  public enum DropAreaType
  {
    DockingManager,
    DocumentPane,
    DocumentPaneGroup,
    AnchorablePane,
  }


  public interface IDropArea
  {
    Rect DetectionRect
    {
      get;
    }
    DropAreaType Type
    {
      get;
    }
  }

  public class DropArea<T> : IDropArea where T : FrameworkElement
  {
    #region Members

    private Rect _detectionRect;
    private DropAreaType _type;
    private T _element;

    #endregion

    #region Constructors

    internal DropArea( T areaElement, DropAreaType type )
    {
      _element = areaElement;
      _detectionRect = areaElement.GetScreenArea();
      _type = type;
    }

    #endregion

    #region Properties

    public Rect DetectionRect
    {
      get
      {
        return _detectionRect;
      }
    }   

    public DropAreaType Type
    {
      get
      {
        return _type;
      }
    }

    public T AreaElement
    {
      get
      {
        return _element;
      }
    }

    #endregion
  }
}
