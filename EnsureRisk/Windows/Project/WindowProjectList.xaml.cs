using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataMapping.Data;
using System.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;
using System.Windows.Input;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowProjectList.xaml
    /// </summary>
    public partial class WindowProjectList : Window
    {
        public bool IS_DELETING { get; set; } = false;
        public DataSet Ds { get; set; }

        public WindowProjectList()
        {
            InitializeComponent();
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

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowProject adduser = new WindowProject
                {
                    DrProject = Ds.Tables[DT_Project.TABLE_NAME].NewRow(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (adduser.ShowDialog() == true)
                {
                    Ds.Tables[DT_Project.TABLE_NAME].Rows.Add(adduser.DrProject);
                    DataRow drWBS = Ds.Tables[DT_WBS.TABLE_NAME].NewRow();
                    drWBS[DT_WBS.IDPROJECT] = adduser.DrProject[DT_Project.ID_PROJECT];
                    drWBS[DT_WBS.NIVEL] = "1";
                    drWBS[DT_WBS.WBS_NAME] = adduser.DrProject[DT_Project.PROJECT_NAME];
                    drWBS[DT_WBS.USERNAME] = "admin";
                    Ds.Tables[DT_WBS.TABLE_NAME].Rows.Add(drWBS);
                    if (Ds.HasChanges())
                    {
                        using (ServiceProject.WebServiceProject ws = new ServiceProject.WebServiceProject())
                        {
                            DataSet temp = Ds.GetChanges();
                            temp = ws.SaveProject(temp);
                            Ds.Merge(temp);
                            Ds.AcceptChanges();
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
                int intcell = dgProject.SelectedIndex;
                if (intcell >= 0)
                {
                    DataRow dr = Ds.Tables[DT_Project.TABLE_NAME].Rows[intcell];
                    WindowProject adduser = new WindowProject
                    {
                        DrProject = dr,
                        Operation = General.UPDATE,
                        Icon = Icon
                    };
                    adduser.ShowDialog();
                    if (adduser.DialogResult == true)
                    {
                        DataSet temp = new DataSet();
                        if (Ds.HasChanges())
                        {
                            ServiceProject.WebServiceProject ws = new ServiceProject.WebServiceProject();
                            temp = Ds.GetChanges();
                            temp = ws.SaveProject(temp);
                            Ds.Merge(temp);
                            Ds.AcceptChanges();
                        }
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
                if (dgProject.SelectedIndex >= 0)
                {
                    Ds.Tables[DT_Project.TABLE_NAME].Rows[dgProject.SelectedIndex].Delete();
                    if (Ds.HasChanges())
                    {
                        using (ServiceProject.WebServiceProject ws = new ServiceProject.WebServiceProject())
                        {
                            DataSet temp = Ds.GetChanges();
                            temp = ws.SaveProject(temp);
                            Ds.Merge(temp);
                            Ds.AcceptChanges();
                        }
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
                if (dgProject.SelectedIndex >= 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Ds.Tables[DT_Project.TABLE_NAME].Rows[dgProject.SelectedIndex][DT_Project.PROJECT_NAME] + "]?");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Ds = new UserDataSet();
                ServiceProject.WebServiceProject webService = new ServiceProject.WebServiceProject();
                Ds.Merge(webService.GetAllProjects());
                dgProject.ItemsSource = Ds.Tables[DT_Project.TABLE_NAME].DefaultView;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void dgProject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
