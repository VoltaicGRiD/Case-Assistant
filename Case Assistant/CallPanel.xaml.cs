using Case_Assistant;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for CallPanel.xaml
    /// </summary>
    public partial class CallPanel : UserControl
    {
        MainWindow mainWindow;

        public CallPanel(string CustomerName, MainWindow mw)
        {
            mainWindow = mw;
            InitializeComponent();
        }

        private string GetData()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append((mainWindow.SRList.SelectedItem as Case).CxName + "\t" + "Call" + "\t" + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + " UTC" + "\t");

            foreach (FrameworkElement element in TypeGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        if ((element as RadioButton).Content.ToString().Contains("Other"))
                            builder.Append(TypeBox.Text + "     ");
                        else
                            builder.Append((element as RadioButton).Content.ToString().Split(' ')[0] + "\t");
                    }
                }
            }

            foreach (FrameworkElement element in AttributeGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        if ((element as RadioButton).Content.ToString().Contains("Other"))
                            builder.Append(TypeBox.Text + "\t");
                        else
                        {
                            switch ((element as RadioButton).Content.ToString())
                            {
                                case "Left Voice-Mail":
                                    builder.Append("LVM\t");
                                    break;
                                case "Wrong Number":
                                    builder.Append("Wrong #\t");
                                    break;
                                case "Unable to leave VM":
                                    builder.Append("No VM\t");
                                    break;
                                default:
                                    builder.Append((element as RadioButton).Content.ToString() + "\t");
                                    break;
                            }
                        }
                    }
                }
            }

            if (RecordedCheck.IsChecked == true)
                builder.Append("RECORDED" + "\t");
            else
                builder.Append("NOT RECORDED" + "\t");

            if (DescBox.Text.Length > 0)
                builder.Append(DescBox.Text);

            return builder.ToString();
        }

        private string GetContent()
        {
            StringBuilder builder = new StringBuilder();

            foreach (FrameworkElement element in TypeGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        if ((element as RadioButton).Content.ToString().Contains("Other"))
                            builder.Append(TypeBox.Text + "\t");
                        else
                            builder.Append((element as RadioButton).Content.ToString().Split(' ')[0] + "\t");
                    }
                }
            }

            foreach (FrameworkElement element in AttributeGrid.Children)
            {
                if (element is RadioButton)
                {
                    if ((element as RadioButton).IsChecked == true)
                    {
                        if ((element as RadioButton).Content.ToString().Contains("Other"))
                            builder.Append(AttriBox.Text + "\t");
                        else
                        {
                            switch ((element as RadioButton).Content.ToString())
                            {
                                case "Left Voice-Mail":
                                    builder.Append("LVM\t");
                                    break;
                                case "Wrong Number":
                                    builder.Append("Wrong #\t");
                                    break;
                                case "Unable to leave VM":
                                    builder.Append("No VM\t");
                                    break;
                                default:
                                    builder.Append((element as RadioButton).Content.ToString() + "\t");
                                    break;
                            }
                        }
                    }
                }
            }

            if (RecordedCheck.IsChecked == true)
                builder.Append("RECORDED" + "\t");
            else
                builder.Append("NOT RECORDED" + "\t");

            if (DescBox.Text.Length > 0)
                builder.Append(DescBox.Text);

            return builder.ToString();
        }

        private void SaveInternalClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetData());
            mainWindow.CallComplete(GetData(), GetContent(), DescBox.Text);
        }

        private void DescBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveInternalClick(this, new RoutedEventArgs());
        }
    }
}
