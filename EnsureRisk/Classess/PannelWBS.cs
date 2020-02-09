using System.Windows.Controls;
using System.Windows;

namespace EnsureRisk.Classess
{
    public class PannelWBS : StackPanel
    {
        public int MyID { get; set; }
        public ButtonWBS BtnEdit { get; set; }
        public ButtonWBS BtnDelete { get; set; }
        public ButtonWBS BtnEye { get; set; }
        public Style MyBtnStyle { get; set; }
        public TextBlock TextBlock { get; set; }
        public string MyText { get; set; }

        public PannelWBS(int MyID) : base()
        {
            this.MyID = MyID;
            BtnEdit = new ButtonWBS
            {
                Style = MyBtnStyle,
                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Pencil },
                ToolTip = "Edit"
            };
            BtnDelete = new ButtonWBS
            {
                Style = MyBtnStyle,
                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Trash },
                ToolTip = "Delete"
            };
            BtnEye = new ButtonWBS
            {
                //Style = ((Button)FindResource("Delete")).Style,
                Style = MyBtnStyle,
                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye },
                ToolTip = "View Diagram"
            };
            TextBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            BtnEye.IdWBS = BtnEdit.IdWBS = BtnDelete.IdWBS = MyID;
            Children.Add(TextBlock);
            Children.Add(new TextBlock { Text = " " });
            Children.Add(BtnEye);
            Children.Add(BtnEdit);
            Children.Add(BtnDelete);
        }


        public void SetText(string text)
        {
            TextBlock.Text = text;
        }

        public void EnableBtns(bool isEnable)
        {
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = isEnable;
        }
    }
}
