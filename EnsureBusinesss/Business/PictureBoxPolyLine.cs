using System.Windows.Controls;
using System.Windows;

namespace EnsureBusinesss.Business
{
    public class PictureBoxPolyLine : Image
    {
        public RiskPolyLine Risk { get; set; }
        public PictureBoxPolyLine() : base()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
            Width = 10;
            Height = 10;
        }
    }
}
