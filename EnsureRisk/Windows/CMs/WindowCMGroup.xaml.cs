using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using DataMapping.Data;
using Microsoft.VisualBasic.CompilerServices;
using EnsureBusinesss;
using EnsureBusinesss.Business;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowCM.xaml
    /// </summary>
    public partial class WindowCMGroup : Window
    {
        public int RiskTreeID { get; set; }
        public DataSet DsCM { get; set; }
        public DataTable MyCM { get; set; }
        //Añadido por roberto
        public DataView DvTopRisk { get; set; }
        public DataTable TopRiskTable { get; set; }
        public decimal Probability { get; set; }
        public string NameShort { get; set; }
        public string Detail { get; set; }

        public WindowCMGroup()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextName, StringResources.ShortNameText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextFather, StringResources.FatherText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDetail, StringResources.RiskDetailText);
            BtnCancel.Content = StringResources.CancelButton;
            //tabRoles.Header = StringResources.TabValues;
            //tabRoles.Header = StringResources.TabRoles;
        }

        private void AcceptCM()
        {
            NameShort = TextName.Text;
            Detail = TextDetail.Text;
            Probability = decimal.Parse(TextProbability.Text) / 100;
            this.DialogResult = true;
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text != "")
                {
                    if (MyCM.Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and " + DT_CounterM.NAMESHORT + " = '" + TextName.Text + "'").Any())
                    {
                        WindowMessageYesNo yesNo = new WindowMessageYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                        if (yesNo.DialogResult == true)
                        {
                            AcceptCM();
                        }
                    }
                    else
                    {
                        AcceptCM();
                    }
                }
                else
                {
                    new WindowMessageOK(StringResources.FIELD_REQUIRED).ShowDialog();
                }
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TextProbability.Text = Probability.ToString();
                TextFather.Text = "various";
                TextName.Text = "";
                TextDetail.Text = "";
                //añadido por Roberto
                DvTopRisk = TopRiskTable.DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;
                DvTopRisk.RowFilter = DT_CounterM_Damage.ID_COUNTERM + " = " + -100;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }

        }
    }
}
