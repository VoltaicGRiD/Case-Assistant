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
    /// Interaction logic for SkypePanel.xaml
    /// </summary>
    public partial class SkypePanel : UserControl
    {
        MainWindow mainWindow;

        public SkypePanel(MainWindow mw)
        {
            mainWindow = mw;
            InitializeComponent();
        }

        private string GetData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Skype Conversation\tWITH\t");

            foreach (FrameworkElement element in WithGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        sb.Append((element as RadioButton).Content);
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine("Conversation Contents");
            sb.Append(ConvoBox.Text);

            return (sb.ToString());
        }

        private string GetWith()
        {
            foreach (FrameworkElement element in WithGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        return (element as RadioButton).Content.ToString();
                    }
                }
            }
            return "";
        }

        private string GetContent()
        {
            return ConvoBox.Text;
        }

        private void SaveToInternalClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetData());
            mainWindow.SkypeConvo(GetWith(), GetData(), GetContent());
        }

        private void ConvoBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveToInternalClick(this, new RoutedEventArgs());
        }
    }
}
