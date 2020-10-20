using EnsureRisk.Resources;
using MaterialDesignColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ThemePage : Page, INotifyPropertyChanged
    {
        public DataTable TableColors { get; set; }
        public const string COLOR = "Color.xml";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
       
        public ThemePage()
        {
            InitializeComponent();
            TableColors = new DataTable();
            ChangeLanguage();
        }
        public void ChangeLanguage()
        {
            //BtnSet.Content = StringResources.SetButton;
        }
       
        private void ChangeColor(string colorName)
        {
            Application.Current.Resources.MergedDictionaries[2].Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." + colorName + ".xaml", UriKind.RelativeOrAbsolute);

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
           
            
            try
            {
                if (File.Exists(COLOR))
                {
                    TableColors.ReadXml(COLOR);
                    if (TableColors.Select().First()["Theme"].ToString() == "Light")
                    {
                        tglTheme.IsChecked = false;
                    }
                    else
                    {
                        tglTheme.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveColors()
        {
            DataRow rowColors = TableColors.NewRow();
            //rowColors["Color"] = cbColors.Text;
            if (tglTheme.IsChecked == true)
            {
                rowColors["Theme"] = "Dark";
            }
            else
            {
                rowColors["Theme"] = "Light";
            }
            //rowColors["StartLogo"] = Togle.IsChecked;
            TableColors.Rows[0].Delete();
            TableColors.Rows.InsertAt(rowColors, 0);
            TableColors.WriteXml(COLOR, XmlWriteMode.WriteSchema);
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveColors();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml", UriKind.RelativeOrAbsolute);
            SaveColors();
        }


        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml", UriKind.RelativeOrAbsolute);
            SaveColors();
        }

        private void Pink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                ChangeColor(btn.Name);
                SaveColors();
            }
        }

    }
}
