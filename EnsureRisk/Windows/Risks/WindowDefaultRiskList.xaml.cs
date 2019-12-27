using EnsureRisk.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataMapping.Data;
using EnsureBusinesss;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowDefaultRiskList.xaml
    /// </summary>
    public partial class WindowDefaultRiskList : Window
    {
        public DataView Dv { get; set; }
        public DataSet Ds { get; set; }

        public WindowDefaultRiskList()
        {
            InitializeComponent();
        }

        public void ChangeLanguage()
        {
           
            Title = StringResources.RiskTitle;
        }

        public void RefreshData()
        {
            try
            {
                Ds = new UserDataSet();
                ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                Ds.Merge(ws.GetDefaultRisk());
                Dv = Ds.Tables[DT_DefaulRisk.Risk_TABLA].DefaultView;
                dgRisk.ItemsSource = Dv;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
            ChangeLanguage();
        }

        private void dgRisk_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (Ds.HasChanges())
            {
                DataSet temp = new DataSet();
                ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                temp = Ds.GetChanges();
                temp = ws.SaveRisk(temp);
                Ds.Merge(temp);
                Ds.AcceptChanges();
                RefreshData();
            }
        }
    }
}
