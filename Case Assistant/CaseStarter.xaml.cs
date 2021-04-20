using CMET_Test;
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

namespace Case_Assistant
{
    /// <summary>
    /// Interaction logic for CaseStarter.xaml
    /// </summary>
    public partial class CaseStarter : Window
    {
        public string SRNum;
        public string BusName;

        MainWindow mw;

        public CaseStarter(MainWindow calledFrom)
        {
            mw = calledFrom;
            InitializeComponent();
        }

        private void BusinessNameEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateClicked(this, new RoutedEventArgs());
            }
        }

        private void CreateClicked(object sender, RoutedEventArgs e)
        {
            if (NewSR.Text.Length > 0 && NewName.Text.Length > 0 && CMETBox.Text.Length == 0)
            {
                SRNum = NewSR.Text;
                BusName = NewName.Text;
                NewSR.Text = "";
                NewName.Text = "";
                mw.CreateCase(SRNum, BusName);
            }
            else if (NewSR.Text.Length == 0 && NewName.Text.Length == 0 && CMETBox.Text.Length > 0)
            {
                string[] data = CMETBox.Text.Split(new string[] { " - " }, StringSplitOptions.None);
                SRNum = data[1];
                BusName = data[2];
                mw.CreateCase(SRNum, BusName);
            }
            else
                MessageBox.Show("Both the SR number and the Business name are required to create a new case.", "Error", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
