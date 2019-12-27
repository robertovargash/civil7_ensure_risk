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
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBSList.xaml
    /// </summary>
    public partial class WindowWBSList : Window
    {
        public DataView Dv { get; set; }
        public DataSet Ds { get; set; }
        public WindowWBSList()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void RefreshData()
        {
            try
            {
                Ds = new UserDataSet();
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                Ds.Merge(ws.GetAllWBS());
                Dv = Ds.Tables[DT_WBS.TABLE_NAME].DefaultView;
                dgWBS.ItemsSource = Dv;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowWBS wbs = new WindowWBS
                {
                    DrWBS = Ds.Tables[DT_WBS.TABLE_NAME].NewRow(),
                    WBS_Structure = Ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                    WBS_Encoder = Ds.Tables[DT_WBS.TABLE_NAME].Copy(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (wbs.ShowDialog() == true)
                {
                    Ds.Tables[DT_WBS.TABLE_NAME].Rows.Add(wbs.DrWBS);
                    Ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                    Ds.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                    if (Ds.HasChanges())
                    {
                        DataSet temp = new DataSet();
                        ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                        temp = Ds.GetChanges();
                        temp = ws.SaveWBS(temp);
                        Ds.Merge(temp);
                        Ds.AcceptChanges();
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBS.SelectedIndex >= 0)
                {
                    DataRow dr = Dv[dgWBS.SelectedIndex].Row;
                    WindowWBS wbs = new WindowWBS
                    {
                        DrWBS = dr,
                        WBS_Structure = Ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                        WBS_Encoder = Ds.Tables[DT_WBS.TABLE_NAME].Copy(),
                        Operation = General.UPDATE,
                        Icon = Icon
                    };
                    if (wbs.ShowDialog() == true)
                    {
                        Ds.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                        Ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                        if (Ds.HasChanges())
                        {
                            DataSet temp = new DataSet();
                            ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                            temp = Ds.GetChanges();
                            temp = ws.SaveWBS(temp);
                            Ds.Merge(temp);
                            Ds.AcceptChanges();
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = Dv[dgWBS.SelectedIndex].Row;
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_WBS.WBS_NAME] + "]?").ShowDialog() == true)
                {
                    foreach (var item in Ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + fila[DT_WBS.ID_WBS]))
                    {
                        Ds.Tables[DT_WBS.TABLE_NAME].Rows.Find(item[DT_WBS_STRUCTURE.ID_CHILD]).Delete();
                    }
                    fila.Delete();
                    if (Ds.HasChanges())
                    {
                        DataSet temp = new DataSet();
                        ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                        temp = Ds.GetChanges();
                        temp = ws.SaveWBS(temp);
                        Ds.Merge(temp);
                        Ds.AcceptChanges();
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void dgRole_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
