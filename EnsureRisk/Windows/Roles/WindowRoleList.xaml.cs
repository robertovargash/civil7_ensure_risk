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
    /// Interaction logic for WindowRoleList.xaml
    /// </summary>
    public partial class WindowRoleList : Window
    {

        public DataView dv { get; set; }
        public DataSet ds { get; set; }
        public DataRow Selected_fila { get; private set; }
        public bool IS_DELETING { get; private set; }

        public WindowRoleList()
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

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void RefreshData()
        {
            try
            {
                ds = new UserDataSet();
                ServiceRoleController.WebServiceRole user = new ServiceRoleController.WebServiceRole();
                ds.Merge(user.GetRolesData());
                dv = ds.Tables[DT_Role.ROLE_TABLE].DefaultView;
                dgRole.ItemsSource = dv;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void ChangeLanguage()
        {
            BtnAdd.Content = StringResources.AddButton;
            BtnEdit.Content = StringResources.EditButton;
            BtnDel.Content = StringResources.DeleteButton;
            Title = StringResources.RoleListMenu;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowRole addrole = new WindowRole
                {
                    dRow = ds.Tables[DT_Role.ROLE_TABLE].NewRow(),
                    OperationTable = ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Copy(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (addrole.ShowDialog() == true)
                {
                    ds.Tables[DT_Role.ROLE_TABLE].Rows.Add(addrole.dRow);
                    ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Merge(addrole.OperationTable);
                    if (ds.HasChanges())
                    {
                        DataSet temp = new DataSet();
                        ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                        temp = ds.GetChanges();
                        temp = ws.SaveRole(temp);
                        ds.Merge(temp);
                        ds.AcceptChanges();
                        RefreshData();
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
                if (dgRole.SelectedItems.Count >= 0)
                {
                    DataRow dr = dv[dgRole.SelectedIndex].Row;
                    if (dr[DT_Role.ROLE_COLUM].ToString() != "Administrator")
                    {
                        WindowRole wr = new WindowRole
                        {
                            dRow = dr,
                            Operation = General.UPDATE,
                            OperationTable = ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Copy(),
                            Icon = Icon
                        };
                        if (wr.ShowDialog() == true)
                        {
                            ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Merge(wr.OperationTable);
                            if (ds.HasChanges())
                            {
                                DataSet temp = new DataSet();
                                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                                temp = ds.GetChanges();
                                temp = ws.SaveRole(temp);
                                ds.Merge(temp);
                                ds.AcceptChanges();
                                RefreshData();
                            }
                        }
                    }
                    else
                    {
                        MostrarErrorDialog("'Administrator' role can´t be changed!");
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
                if (fila[DT_Role.ROLE_COLUM].ToString() != "Administrator")
                {
                    fila.Delete();
                    if (ds.HasChanges())
                    {
                        using (ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole())
                        {
                            DataSet temp = ds.GetChanges();
                            temp = ws.SaveRole(temp);
                            ds.Merge(temp);
                            ds.AcceptChanges();
                            RefreshData();
                        }
                    }
                }
                else
                {
                    MostrarErrorDialog("'Administrator' role can´t be deleted!");
                }
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
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
                    Delete(Selected_fila);
                }
            }
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selected_fila = dv[dgRole.SelectedIndex].Row;
                IS_DELETING = true;
                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_fila[DT_Role.ROLE_COLUM] + "]?");
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void dgRole_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
