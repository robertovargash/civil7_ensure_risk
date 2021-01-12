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
using System.ComponentModel;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowRole.xaml
    /// </summary>
    public partial class WindowRole : Window, INotifyPropertyChanged
    {
        private string _role;
        public string Role { get { return _role; } set { _role = value; OnPropertyChanged("Role"); } }
    
        public bool IS_DELETING { get; set; } = false;
        public string Operation { get; set; }
        public DataTable OperationTable { get; set; }
        public DataView dv { get; set; }
        public DataRow dRow { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowRole()
        {
            InitializeComponent();
            ChangeLanguage();
            TextRole.DataContext = this;
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
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
                DataTable OpCod = ws.GetOperation().Tables[DT_Operation.TABLE_NAME].Copy();
                WindowSelection frm = new WindowSelection
                {
                    Dt = General.DeleteExists(OpCod, OperationTable, DT_Operation.ID_COLUM),
                    DcolumToShow = new string[] { DT_Operation.OPERATION_COLUMN },
                    DcolumToShowAlias = new string[] { DT_Operation.OPERATION_COLUMN },
                    Title = "Operation",
                    FilterString = "Operation Name",
                    ColumnToFilter = DT_Operation.OPERATION_COLUMN
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
                MostrarErrorDialog(ex.Message);
            }

        }

        private void Delete()
        {
            try
            {
                if (dgOperation.SelectedIndex >= 0)
                    OperationTable.Rows[dgOperation.SelectedIndex].Delete();
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
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
                if (dgOperation.SelectedIndex >= 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + OperationTable.Rows[dgOperation.SelectedIndex][DT_OperationRole.OPERATION_COLUMN] + "]?");
                }
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
                    Delete();
                }
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
                MostrarErrorDialog(ex.Message);
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
                    OperationTable = ws.GetRolesOperation(new object[] { (Int32)dRow[DT_Role.IDROL_COLUMN] }).Tables[DT_OperationRole.TABLE_NAME].Copy();
                    dv = OperationTable.DefaultView;
                    TextRole.Text = dRow[DT_Role.ROLE_COLUM].ToString();
                    dgOperation.ItemsSource = dv;
                }
                catch (Exception ex)
                {
                    MostrarErrorDialog(ex.Message);
                }
            }
            TextRole.Focus();
        }

        private void TextRole_TextChanged(object sender, TextChangedEventArgs e)
        {
            Role = TextRole.Text;
        }
    }
}
