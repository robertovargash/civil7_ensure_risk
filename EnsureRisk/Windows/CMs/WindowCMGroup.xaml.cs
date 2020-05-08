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
        public bool IS_USING_NAME { get; set; } = false;
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

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        private void YesNoDialog_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                return;
            }
            if (Equals(eventArgs.Parameter, true))
            {
                if (IS_USING_NAME)
                {
                    AcceptCM();
                }
            }
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
            IS_USING_NAME = false;
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
                        IS_USING_NAME = true;
                        MostrarDialogYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");                        
                    }
                    else
                    {
                        AcceptCM();
                    }
                }
                else
                {
                    MostrarErrorDialog(StringResources.FIELD_REQUIRED);
                }
            }
            catch (Exception EX)
            {
                IS_USING_NAME = false;
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
                MostrarErrorDialog(ex.Message);
            }

        }
    }
}
