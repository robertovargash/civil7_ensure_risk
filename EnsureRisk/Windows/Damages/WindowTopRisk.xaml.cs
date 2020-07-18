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
using System.ComponentModel;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowTopRisk.xaml
    /// </summary>
    public partial class WindowTopRisk : Window, INotifyPropertyChanged
    {
        private string damage, um;
        public string DAMAGE { get { return damage; } set { damage = value; OnPropertyChanged("DAMAGE"); } }
        public string UM { get { return um; } set { um = value; OnPropertyChanged("UM"); } }

        public DataRow Drow { get; set; }
        public string Operation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public WindowTopRisk()
        {
            InitializeComponent();
            ChangeLanguage();
            TextTopRisk.DataContext = this;
            UMText.DataContext = this;
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
                    DAMAGE = Drow[DT_Damage.TOP_RISK_COLUMN].ToString();
                    UM= Drow[DT_Damage.UM].ToString();
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
                using (ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk())
                {
                    if (ws.GetAllTopRisk().Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DAMAGE + "' and " + DT_Damage.UM + " = '" + UM + "'").Any())
                    {
                        MostrarErrorDialog("This damage exists with the UM. Insert other.");
                    }
                    else
                    {
                        Drow[DT_Damage.COLORID_COLUMNA] = colorPiker.SelectedColor.ToString();
                        Drow[DT_Damage.TOP_RISK_COLUMN] = DAMAGE;
                        Drow[DT_Damage.UM] = UM;
                        DialogResult = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextTopRisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DAMAGE = TextTopRisk.Text;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void UMText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UM = UMText.Text;
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
