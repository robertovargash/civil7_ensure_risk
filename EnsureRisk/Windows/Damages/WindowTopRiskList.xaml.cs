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
        public bool IS_DELETING { get; set; } = false;
        public DataRow Selected_DR { get; set; }
        public DataView Dv { get; set; }
        public DataSet Ds { get; set; }
        public string Operation { get; set; }
        public WindowTopRiskList()
        {
            InitializeComponent();
            ChangeLanguage();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
                if (IS_DELETING)
                {
                    Delete(Selected_DR);
                }
            }
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
                using (ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk())
                {
                    Ds = new UserDataSet();
                    Ds.Merge(ws.GetAllTopRisk());
                    Dv = Ds.Tables[DT_Damage.TABLE_NAME].DefaultView;
                    dgTopRisk.ItemsSource = Dv;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowTopRisk formTop = new WindowTopRisk
                {
                    Drow = Ds.Tables[DT_Damage.TABLE_NAME].NewRow(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (formTop.ShowDialog() == true)
                {
                    Ds.Tables[DT_Damage.TABLE_NAME].Rows.Add(formTop.Drow);
                    if (Ds.HasChanges())
                    {
                        using (ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk())
                        {
                            DataSet temp = Ds.GetChanges();
                            temp = ws.SaveTopRisk(temp);
                            Ds.Merge(temp);
                            Ds.AcceptChanges();
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTopRisk.SelectedIndex >= 0)
                {
                    DataRow dr = Dv[dgTopRisk.SelectedIndex].Row;
                    WindowTopRisk windowTop = new WindowTopRisk
                    {
                        Drow = dr,
                        Operation = General.UPDATE,
                        Icon = Icon
                    };
                    if (windowTop.ShowDialog() == true)
                    {
                        if (Ds.HasChanges())
                        {
                            using (ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk())
                            {
                                DataSet temp = Ds.GetChanges();
                                temp = ws.SaveTopRisk(temp);
                                Ds.Merge(temp);
                                Ds.AcceptChanges();
                                RefreshData();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete(DataRow fila)
        {
            try
            {
                fila.Delete();
                if (Ds.HasChanges())
                {
                    using (ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk())
                    {
                        DataSet temp = Ds.GetChanges();
                        temp = ws.SaveTopRisk(temp);
                        Ds.Merge(temp);
                        Ds.AcceptChanges();
                        RefreshData();
                    }
                }
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTopRisk.SelectedIndex >= 0)
                {
                    Selected_DR = Dv[dgTopRisk.SelectedIndex].Row;
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_DR[DT_Damage.TOP_RISK_COLUMN] + "]?");
                }                
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }

        }

        private void dgTopRisk_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
