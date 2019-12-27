using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using DataMapping.Data;
using System.Windows.Media;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowTopRisk.xaml
    /// </summary>
    public partial class WindowTopRisk : Window
    {
        public DataRow Drow { get; set; }
        public string Operation { get; set; }

        private System.Drawing.Color color;
        public WindowTopRisk()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextTopRisk, StringResources.TopRiskDefinitionText);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.TopRiskTitle;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.UPDATE)
                {
                    TextTopRisk.Text = Drow[DT_Damage.TOP_RISK_COLUMN].ToString();
                    int colorete = int.Parse(Drow[DT_Damage.COLORID_COLUMNA].ToString());
                    color = System.Drawing.Color.FromArgb(colorete);
                    colorPiker.SelectedColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                }
                TextTopRisk.Focus();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                color = System.Drawing.Color.FromArgb(colorPiker.SelectedColor.Value.A, colorPiker.SelectedColor.Value.R, colorPiker.SelectedColor.Value.G, colorPiker.SelectedColor.Value.B);
                Drow[DT_Damage.COLORID_COLUMNA] = color.ToArgb().ToString();
                Drow[DT_Damage.TOP_RISK_COLUMN] = TextTopRisk.Text;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //private void BtnColor_Click(object sender, RoutedEventArgs e)
        //{
        //    //ColorDialog dlg = new ColorDialog();
        //    //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    //{
        //    //    BtnColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
        //    //    color = dlg.Color;
        //    //}
        //    WindowColorPicker wc = new WindowColorPicker();
        //    if (Operation == General.UPDATE)
        //    {
        //        wc.R = color.R;
        //        wc.G = color.G;
        //        wc.B = color.B;
        //        Operation = General.UPDATE;
        //    }
        //    wc.ShowDialog();
        //    if (wc.DialogResult == true)
        //    {
        //        BtnColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, wc.R, wc.G, wc.B));
        //        color = System.Drawing.Color.FromArgb(255, wc.R, wc.G, wc.B);
        //    }
        //}
    }
}
