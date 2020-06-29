using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Data;
using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;
using EnsureRisk.Windows.Damages;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowTreeRisk.xaml
    /// </summary>
    public partial class WindowTreeRisk : Window, INotifyPropertyChanged
    {
        private string _name;
        public string DName { get { return _name; } set { _name = value; OnPropertyChanged("DName"); } }
        public bool IS_DELETING { get; set; } = false;
        public string Operation { get; set; }
        public int IDProject { get; set; }
        public DataTable TopRiskTable { get; set; }
        public DataTable CM_TopRisk { get; set; }
        public DataTable Risk_TopRisk { get; set; }
        public DataRow DRow { get; set; }
        public DataView Dv { get; set; }
        public string SuggestionName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowTreeRisk()
        {
            InitializeComponent();
            ChangeLanguage();
            TextName.DataContext = this;
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextName, StringResources.DiagramNameLabel);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.RiskDiagramTitle;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.UPDATE)
                {
                    TextName.Text = DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                }
                else
                {
                    TextName.Text = SuggestionName;
                }
                Dv = TopRiskTable.DefaultView;
                Dv.RowFilter = DT_Diagram_Damages.ID_RISKTREE + " = " + DRow[DT_Diagram.ID_DIAGRAM];
                dgTopRisk.ItemsSource = Dv;
                TextName.Focus();
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
                WindowSelectDamageDiagram frm = new WindowSelectDamageDiagram
                {
                    Drow = TopRiskTable.NewRow(),
                    Icon = Icon,
                    DamageDiagramTable = TopRiskTable.Copy()
                };
                if (frm.ShowDialog() == true)
                {
                    frm.Drow[DT_Diagram_Damages.ID_RISKTREE] = DRow[DT_Diagram.ID_DIAGRAM];
                    frm.Drow[DT_Diagram_Damages.RISK_TREE] = DRow[DT_Diagram.DIAGRAM_NAME];
                    TopRiskTable.Rows.Add(frm.Drow);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DELETE_DAMAGE()
        {
            if (dgTopRisk.SelectedIndex >= 0)
            {
                if (Operation == General.UPDATE)
                {
                    foreach (DataRow item in Risk_TopRisk.
                    Select(DT_Risk_Damages.ID_DAMAGE + " = " + TopRiskTable.Rows[dgTopRisk.SelectedIndex][DT_Diagram_Damages.ID_DAMAGE] +
                    " and " + DT_Risk_Damages.ID_RISK_TREE + " = " + DRow[DT_Diagram.ID_DIAGRAM]))
                    {
                        item.Delete();
                    }
                    foreach (DataRow itemi in CM_TopRisk.
                        Select(DT_CounterM_Damage.ID_DAMAGE + " = " + TopRiskTable.Rows[dgTopRisk.SelectedIndex][DT_Diagram_Damages.ID_DAMAGE] +
                        " and " + DT_CounterM_Damage.ID_RISK_TREE + " = " + DRow[DT_Diagram.ID_DIAGRAM]))
                    {
                        itemi.Delete();
                    }
                }
                Dv.Delete(dgTopRisk.SelectedIndex);
            }
            IS_DELETING = false;
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = Dv[dgTopRisk.SelectedIndex].Row;
                if (dgTopRisk.SelectedIndex >= 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_Diagram_Damages.DAMAGE] + "]?");
                }
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TopRiskTable.Select(DT_Diagram_Damages.ID_RISKTREE + " = " + DRow[DT_Diagram.ID_DIAGRAM]).Any())
                {
                    if (TextName.Text != "")
                    {
                        DRow[DT_Diagram.DIAGRAM_NAME] = TextName.Text;
                        DRow[DT_Diagram.ID_PROJECT] = IDProject;
                        this.DialogResult = true;
                    }
                    else
                    {
                        MostrarErrorDialog(StringResources.FIELD_REQUIRED);
                    }
                }
                else
                {
                    MostrarErrorDialog("You must select at least one Damage!!!");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
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
                    DELETE_DAMAGE();
                }
            }
        }

        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {
            DName = TextName.Text;

        }
    }
}
