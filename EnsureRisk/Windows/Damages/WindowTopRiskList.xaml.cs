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
    /// Interaction logic for WindowTopRiskList.xaml
    /// </summary>
    public partial class WindowTopRiskList : Window
    {
        public DataView dv { get; set; }
        public DataSet ds { get; set; }
        public string Operation { get; set; }
        public WindowTopRiskList()
        {
            InitializeComponent();
            ChangeLanguage();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            BtnAdd.Content = StringResources.AddButton;
            BtnEdit.Content = StringResources.EditButton;
            BtnDel.Content = StringResources.DeleteButton;
            Title = StringResources.TopRiskList;
        }

        public void RefreshData()
        {
            try
            {
                ds = new UserDataSet();
                ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                ds.Merge(ws.GetAllTopRisk());
                dv = ds.Tables[DT_Damage.TopRisk_TABLA].DefaultView;
                dgTopRisk.ItemsSource = dv;
                //for (int i = 0; i < dgTopRisk.Items.Count; i++)
                //{
                //    Color colorcito = Color.FromArgb(int.Parse(ds.Tables[TopRiskDatos.TopRisk_TABLA].Rows[i][TopRiskDatos.COLORID_COLUMNA].ToString()));
                //    dgTopRisk.Items[i]
                //}
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowTopRisk formTop = new WindowTopRisk
                {
                    Drow = ds.Tables[DT_Damage.TopRisk_TABLA].NewRow(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (formTop.ShowDialog() == true)
                {
                    ds.Tables[DT_Damage.TopRisk_TABLA].Rows.Add(formTop.Drow);
                    if (ds.HasChanges())
                    {
                        DataSet temp = new DataSet();
                        ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                        temp = ds.GetChanges();
                        temp = ws.SaveTopRisk(temp);
                        ds.Merge(temp);
                        ds.AcceptChanges();
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTopRisk.SelectedIndex >= 0)
                {
                    DataRow dr = dv[dgTopRisk.SelectedIndex].Row;
                    WindowTopRisk windowTop = new WindowTopRisk
                    {
                        Drow = dr,
                        Operation = General.UPDATE,
                        Icon = Icon
                    };
                    if (windowTop.ShowDialog() == true)
                    {
                        if (ds.HasChanges())
                        {
                            DataSet temp = new DataSet();
                            ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                            temp = ds.GetChanges();
                            temp = ws.SaveTopRisk(temp);
                            ds.Merge(temp);
                            ds.AcceptChanges();
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = dv[dgTopRisk.SelectedIndex].Row;
                //if (MessageBox.Show(StringResources.DELETE_MESSAGE, General.DELETE_HEADER, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                //    .Equals(DialogResult.Yes))
                 if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_Damage.TOP_RISK_COLUMN] + "]?").ShowDialog() == true)
                {
                    fila.Delete();
                    if (ds.HasChanges())
                    {
                        DataSet temp = new DataSet();
                        ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                        temp = ds.GetChanges();
                        temp = ws.SaveTopRisk(temp);
                        ds.Merge(temp);
                        ds.AcceptChanges();
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }

        }

        private void dgTopRisk_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
