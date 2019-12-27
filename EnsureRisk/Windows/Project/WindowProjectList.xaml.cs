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
        public DataSet Ds { get; set; }

        public WindowProjectList()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
                    DataSet temp = new DataSet();
                    Ds.Tables[DT_Project.TABLE_NAME].Rows.Add(adduser.DrProject);
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
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intcell = dgProject.SelectedIndex;
                if (intcell >= 0)
                {
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + Ds.Tables[DT_Project.TABLE_NAME].Rows[intcell][DT_Project.PROJECT_NAME] + "]?").ShowDialog() == true)
                    {
                        Ds.Tables[DT_Project.TABLE_NAME].Rows[intcell].Delete();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void dgProject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
