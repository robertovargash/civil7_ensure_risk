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
//using System.Drawing;
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

        public WindowTopRisk()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
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
                    colorPiker.SelectedColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(Drow[DT_Damage.COLORID_COLUMNA].ToString())).Color;
                }
                TextTopRisk.Focus();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //color = System.Drawing.Color.FromArgb(colorPiker.SelectedColor.Value.A, colorPiker.SelectedColor.Value.R, colorPiker.SelectedColor.Value.G, colorPiker.SelectedColor.Value.B);
                Drow[DT_Damage.COLORID_COLUMNA] = colorPiker.SelectedColor.ToString();
                Drow[DT_Damage.TOP_RISK_COLUMN] = TextTopRisk.Text;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
