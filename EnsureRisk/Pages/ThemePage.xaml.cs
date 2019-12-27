using EnsureRisk.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EnsureRisk.Pages
{
    /// <summary>
    /// Interaction logic for TopRiskPage.xaml
    /// </summary>
    public partial class ThemePage : Page
    {
        public DataTable TableColors { get; set; }
        public const string COLOR = "Color.xml";
        public ThemePage()
        {
            InitializeComponent();
            TableColors = new DataTable();
            ChangeLanguage();
        }
        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(ColorThemeText, StringResources.ColorThemeLabel);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(ColorContentText, StringResources.ColorContentLabel);
            BtnSet.Content = StringResources.SetButton;
        }
        private void cbTheme_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string Theme = (String)cbTheme.SelectedItem;
                Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme." + Theme + ".xaml", UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cbColors_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string Color = (String)cbColors.SelectedItem;
                Application.Current.Resources.MergedDictionaries[2].Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." + Color + ".xaml", UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> ListTheme = new List<string>
            {
                "Dark","Light"
            };
            List<string> ListColor = new List<string>
            {
                "Amber",
                "Blue",
                "Brown",
                "Cyan",
                "DeepOrange",
                "DeepPurple",
                "Green",
                "Indigo",
                "LightBlue",
                "LightGreen",
                "Lime",
                "Orange",
                "Pink",
                "Purple",
                "Red",
                "Teal",
                "Yellow"
            };
            cbTheme.ItemsSource = ListTheme;
            cbColors.ItemsSource = ListColor;
            
            try
            {
                if (File.Exists(COLOR))
                {
                    TableColors.ReadXml(COLOR);
                    cbTheme.Text = TableColors.Select().First()["Theme"].ToString();
                    cbColors.Text = TableColors.Select().First()["Color"].ToString();
                    Togle.IsChecked = (Boolean)TableColors.Select().First()["StartLogo"];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow rowColors = TableColors.NewRow();
                rowColors["Color"] = cbColors.Text;
                rowColors["Theme"] = cbTheme.Text;
                rowColors["StartLogo"] = Togle.IsChecked;
                TableColors.Rows[0].Delete();
                TableColors.Rows.InsertAt(rowColors, 0);
                TableColors.WriteXml(COLOR, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
