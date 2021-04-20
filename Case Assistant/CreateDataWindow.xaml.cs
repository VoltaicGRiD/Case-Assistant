using CMET_Test;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CreateDataWindow.xaml
    /// </summary>
    public partial class CreateDataWindow : Window
    {
        MainWindow main;
        Case CaseData;

        public CreateDataWindow(MainWindow window)
        {
            main = window;
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            CaseData = main.SRList.SelectedItem as Case;
            string cxName = "";
            File.WriteAllText("C:\\Users\\" + Environment.UserName + "\\Documents\\" + (main.SRList.SelectedItem as Case).SRNum + ".txt", DataBox.Text);
            string[] Data = File.ReadAllLines("C:\\Users\\" + Environment.UserName + "\\Documents\\" + (main.SRList.SelectedItem as Case).SRNum + ".txt");
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] == "TAM" && Data[i + 2] == "TAM")
                    CaseData.TAM = Data[i + 3];
                if (Data[i] == "TAM Backup" && Data[i + 2] == "TAM Backup")
                    CaseData.BTAM = Data[i + 3];
                if (Data[i] == "MS Product" && Data[i + 2] == "MS Product")
                    CaseData.Tech = Data[i + 3];
                if (Data[i] == "Security Issue" && Data[i + 2] == "Security Issue")
                {
                    if (Data[i + 3] == "Yes")
                        CaseData.Security = true;
                    else
                        CaseData.Security = false;
                }
                if (Data[i] == "Compromised" && Data[i + 2] == "Compromised")
                {
                    if (Data[i + 3] == "Yes")
                        CaseData.DataComp = true;
                    else
                        CaseData.DataComp = false;
                }
                if (Data[i] == "PG Engaged?" && Data[i + 2] == "PG Engaged?")
                {
                    if (Data[i + 3] == "Yes")
                        CaseData.PG = true;
                    else
                        CaseData.PG = false;
                }
                if (Data[i] == "Customer Engaged?" && Data[i + 2] == "Customer Engaged?")
                {
                    if (Data[i + 3] == "Yes")
                        CaseData.CustomerEngaged = true;
                    else
                        CaseData.CustomerEngaged = false;
                }
                if (Data[i] == "CRITSIT Additional Contacts ( Enter the emails separated by ; )" && Data[i + 2] == "CRITSIT Additional Contacts ( Enter the emails separated by ; )")
                    CaseData.Contacts = Data[i + 3];
                if (Data[i] == "Overview" && Data[i + 2] == "Overview")
                    CaseData.OV = Data[i + 3];
                if (Data[i] == "Business Impact" && Data[i + 2] == "Business Impact")
                    CaseData.BI = Data[i + 3];
                if (Data[i] == "Current Status (Customer Ready)" && Data[i + 2] == "Current Status (Customer Ready)")
                    CaseData.CS = Data[i + 3];
                //if (Data[i] == "Action Plan (Customer Ready)" && Data[i + 2] == "Action Plan (Customer Ready)")
                    //CaseData.AP = Data[i + 3];
                //if (Data[i] == "MSinternal Only" && Data[i + 2] == "MSinternal Only")
                    //CaseData.Internal = Data[i + 3];
                if (Data[i] == "SR Case Owner" && Data[i + 2] == "SR Case Owner")
                    CaseData.Engi = Data[i + 3];
                if (Data[i] == "Primary Email" && Data[i + 2] == "Primary Email")
                    CaseData.EngiAlias = Data[i + 3].Replace("@microsoft.com", "");
                if (Data[i] == "Case First Name" && Data[i + 2] == "Case First Name")
                    cxName = Data[i + 3];
                if (Data[i] == "Case Last Name" && Data[i + 2] == "Case Last Name")
                    cxName += " " + Data[i + 3];
                if (Data[i] == "Case Phone Number 1 :: Case Phone Number" && Data[i + 2] == "Case Phone Number 1 :: Case Phone Number")
                    CaseData.CxPhone = Data[i + 3];

                DataBox.Text = "";
                this.Close();
            }
        }
    }
}
