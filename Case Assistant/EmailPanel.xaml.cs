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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Case_Assistant
{
    /// <summary>
    /// Interaction logic for EmailPanel.xaml
    /// </summary>
    public partial class EmailPanel : UserControl
    {
        string CxName;
        MainWindow mainWindow;

        public EmailPanel(string CustomerName, MainWindow mw)
        {
            CxName = CustomerName;
            mainWindow = mw;
            InitializeComponent();
        }

        private string GetData()
        {
            StringBuilder sb = new StringBuilder();

            foreach(FrameworkElement element in ToGrid.Children)
            {
                if (element is RadioButton && (element as RadioButton).IsChecked == true)
                {
                    if (!(element as RadioButton).Content.ToString().Contains("Other") && !(element as RadioButton).Content.ToString().Contains("Customer"))
                        sb.Append((element as RadioButton).Content.ToString());
                    else if ((element as RadioButton).Content.ToString().Contains("Customer"))
                        sb.Append((mainWindow.SRList.SelectedItem as Case).CxName.Split(' ')[0]);
                    else if ((element as RadioButton).Content.ToString().Contains("Other"))
                        sb.Append(ToOtherBox.Text);
                }
            }

            sb.Append("\t" + "E-Mail" + "\t" + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + " UTC" + "\t");

            foreach (FrameworkElement element in TypeGrid.Children)
            {
                if (element is RadioButton && (element as RadioButton).IsChecked == true)
                {
                    if (!(element as RadioButton).Content.ToString().Contains("Other"))
                        sb.Append((element as RadioButton).Content + "\t");
                    else if ((element as RadioButton).Content.ToString().Contains("Other"))
                        sb.Append(TypeOtherBox.Text + "\t");    
                }
            }

            sb.Append(DescBox.Text);
            return sb.ToString();
        }

        private string GetContent()
        {
            return DescBox.Text;
        }

        private void SaveInternalClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetData());
            mainWindow.EmailSent(GetData(), GetContent(), GetContent());
        }

        private void DescBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveInternalClick(this, new RoutedEventArgs());
        }
    }
}
