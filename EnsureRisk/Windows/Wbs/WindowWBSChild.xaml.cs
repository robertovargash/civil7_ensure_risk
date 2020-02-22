using DataMapping.Data;
using System;
using System.Data;
using System.Windows;


namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBSChild.xaml
    /// </summary>
    public partial class WindowWBSChild : Window
    {
        public DataRow DrWBS { get; set; }
        public int Cantidad { get; set; }
        public WindowWBS MyFather { get; set; }
        public DataRow DrWBS_Structure { get; set; }
        public int IdProject { get; set; }
        public WindowWBSChild()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceUserController.WebServiceUser wsu = new ServiceUserController.WebServiceUser();
            DataTable OpCod = wsu.GetUserData().Tables[DT_User.User_TABLA].Copy();
            cbUser.ItemsSource = OpCod.DefaultView;
            cbUser.SelectedValuePath = DT_User.USERNAME;
            cbUser.DisplayMemberPath = DT_User.USERNAME;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrWBS_Structure[DT_WBS_STRUCTURE.ID_CHILD] = DrWBS[DT_WBS.ID_WBS];
                DrWBS_Structure[DT_WBS_STRUCTURE.CHILD] = TextName.Text;
                DrWBS_Structure[DT_WBS_STRUCTURE.CNIVEL] = TextLevel.Text;
                DrWBS_Structure[DT_WBS_STRUCTURE.CHILD_USER] = cbUser.Text;
                DrWBS[DT_WBS.WBS_NAME] = TextName.Text;
                DrWBS[DT_WBS.NIVEL] = TextLevel.Text;
                DrWBS[DT_WBS.IDPROJECT] = IdProject;
                DrWBS[DT_WBS.USERNAME] = cbUser.Text;


                MyFather.WBS_Encoder.Rows.Add(DrWBS);
                MyFather.WBS_Structure.Rows.Add(DrWBS_Structure);

                DrWBS = MyFather.WBS_Encoder.NewRow();
                DrWBS_Structure = MyFather.WBS_Structure.NewRow();
                DrWBS_Structure[DT_WBS_STRUCTURE.ID_FATHER] = MyFather.DrWBS[DT_WBS.ID_WBS];
                DrWBS_Structure[DT_WBS_STRUCTURE.FATHER] = MyFather.DrWBS[DT_WBS.WBS_NAME];
                Cantidad++;
                TextLevel.Text = MyFather.TextLevel.Text + "." + Cantidad;
                TextName.Clear();
                cbUser.SelectedIndex = -1;
                TextName.Focus();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
