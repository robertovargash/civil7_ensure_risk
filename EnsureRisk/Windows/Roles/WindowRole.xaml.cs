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
    /// Interaction logic for WindowRole.xaml
    /// </summary>
    public partial class WindowRole : Window
    {
        public string Operation { get; set; }
        public DataTable OperationTable { get; set; }
        public DataView dv { get; set; }
        public DataRow dRow { get; set; }
        public WindowRole()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextRole, StringResources.RoleNameText);
            BtnCancel.Content = StringResources.CancelButton;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable OpCod = ws.GetOperation().Tables[DT_Operation.OPERATION_TABLE].Copy();
                WindowSelection frm = new WindowSelection
                {
                    dt = General.DeleteExists(OpCod, OperationTable, DT_Operation.ID_COLUM),
                    dcolumToShow = new string[] { DT_Operation.OPERATION_COLUMN },
                    dcolumToShowAlias = new string[] { DT_Operation.OPERATION_COLUMN },
                    Title = "Operation"
                };
                if (frm.ShowDialog() == true)
                {
                    foreach (DataRow item in frm.RowsSelected)
                    {
                        DataRow newrow = OperationTable.NewRow();
                        newrow[DT_OperationRole.ID_OPERATION_COLUM] = (Int32)item[DT_Operation.ID_COLUM];
                        newrow[DT_OperationRole.OPERATION_COLUMN] = item[DT_Operation.OPERATION_COLUMN];
                        newrow[DT_OperationRole.ROLE_COLUM] = TextRole.Text;
                        newrow[DT_OperationRole.IDROL_COLUMN] = dRow[DT_Role.IDROL_COLUMN];
                        OperationTable.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgOperation.SelectedIndex >= 0)
                {
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + OperationTable.Rows[dgOperation.SelectedIndex][DT_OperationRole.OPERATION_COLUMN] + "]?").ShowDialog() == true)
                        OperationTable.Rows[dgOperation.SelectedIndex].Delete();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dRow[DT_Role.ROLE_COLUM] = TextRole.Text;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Operation == General.INSERT)
            {
                dv = OperationTable.DefaultView;
                dgOperation.ItemsSource = dv;
            }

            if (this.Operation == General.UPDATE)
            {
                try
                {
                    ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                    OperationTable = ws.GetRolesOperation(new object[] { (Int32)dRow[DT_Role.IDROL_COLUMN] }).Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Copy();
                    dv = OperationTable.DefaultView;
                    TextRole.Text = dRow[DT_Role.ROLE_COLUM].ToString();
                    dgOperation.ItemsSource = dv;
                }
                catch (Exception ex)
                {
                    new WindowMessageOK(ex.Message).ShowDialog();
                }
            }
            TextRole.Focus();
        }
    }
}
