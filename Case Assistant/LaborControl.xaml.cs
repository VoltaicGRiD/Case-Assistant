using CMET_Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Case_Assistant
{
    /// <summary>
    /// Interaction logic for LaborControl.xaml
    /// </summary>
    public partial class LaborControl : UserControl
    {
        MainWindow main;

        public LaborControl(MainWindow window)
        {
            main = window;
            this.DataContext = main.SRList.SelectedItem;
            InitializeComponent();
            
        }

        public void ChangeDataContext()
        {
            LaborView.DataContext = main.SRList.SelectedItem as Case;
            MainGrid.DataContext = main.SRList.SelectedItem as Case;
        }

        private void LaborComboChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ComboBoxItem item in e.AddedItems)
            {
                if ((string)item.Content == "Other")
                    OtherBox.IsEnabled = true;
            }
        }

        private void AddLaborClick(object sender, RoutedEventArgs e)
        {
            if ((string)(LaborCombo.SelectedItem as ComboBoxItem).Content != "Other")
                (main.SRList.SelectedItem as Case)._Labor.Add(new Labors() { LaborActivity = (LaborCombo.SelectedItem as ComboBoxItem).Content.ToString(), LaborLoggedAt = DateTime.Now, LaborTime = (int)TimeChanger.Value });
            else
                (main.SRList.SelectedItem as Case)._Labor.Add(new Labors() { LaborActivity = OtherBox.Text, LaborLoggedAt = DateTime.Now, LaborTime = (int)TimeChanger.Value });
            foreach (Labors l in (main.SRList.SelectedItem as Case).Labor)
            {
                Debug.WriteLine(l.LaborActivity + " -- " + l.LaborTime);
            }
        }

        private void CopyLaborClick(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Labors l in LaborView.Items)
            {
                sb.AppendLine(l.LaborActivity + " -- " + l.LaborTime + " Minutes -- Logged at: " + l.LaborLoggedAt.ToString("hh:mm tt"));
            }

            Clipboard.SetText(sb.ToString());
        }

        private void CalcLaborClick(object sender, RoutedEventArgs e)
        {
            int TotalLabor = 0;
            foreach (Labors l in LaborView.Items)
            {
                TotalLabor += l.LaborTime;
            }
            TotalLaborBox.Text = TotalLabor.ToString();
        }
    }
}
