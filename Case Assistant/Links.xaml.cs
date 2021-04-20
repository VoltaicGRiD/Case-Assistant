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
    /// Interaction logic for Links.xaml
    /// </summary>
    public partial class Links : UserControl
    {
        string SRNumber, BusinessName;
        string BI, CS, AP;

        public Links(string SRNum, string BusImp, string CurStat, string ActPln, string BusName)
        {
            SRNumber = SRNum;
            BusinessName = BusName;
            BI = BusImp;
            CS = CurStat;
            AP = ActPln;
            InitializeComponent();
        }

        private void PBClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://critsit.crm.dynamics.com/main.aspx?etc=10048&extraqs=%3fetc%3d10048&histKey=660689373&newWindow=true&pagetype=entityrecord#921143326");
        }

        private void SDClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://servicedesk.microsoft.com/#/customer/commercial?caseNumber=" + SRNumber);
        }

        private void PortalClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://critsit.crm.dynamics.com/main.aspx#792162642");
        }

        private void IntroClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("Hello " + CSMNameBox.Text + ", I'm the CSM for SR# " + SRNumber + " - " + BusinessName + " :) How are you?");
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            if (CSMNameBox.Text == "Their First Name")
                CSMNameBox.Text = "";
        }

        private void TeamsClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://teams.microsoft.com/l/entity/com.microsoft.teamspace.tab.planner/_djb2_msteams_prefix_2142145758?webUrl=https%3a%2f%2ftasks.office.com%2fmicrosoft.onmicrosoft.com%2fHome%2fPlanViews%2f5DE9a5GowE-r9pk7C5GLXpUADHTF&label=Escalations%2fResource+Issues&context=%7b%0d%0a++%22subEntityId%22%3a+null%2c%0d%0a++%22canvasUrl%22%3a+%22https%3a%2f%2ftasks.office.com%2fmicrosoft.onmicrosoft.com%2fHome%2fPlannerFrame%3fpage%3d7%26planId%3d5DE9a5GowE-r9pk7C5GLXpUADHTF%26auth_pvr%3dOrgid%26auth_upn%3d%7bupn%7d%26mkt%3d%7blocale%7d%22%2c%0d%0a++%22channelId%22%3a+%2219%3ac6b9db685f4c4a21b7a0cd874d574723%40thread.skype%22%0d%0a%7d&groupId=ff4a4b74-a580-4cb9-b47d-7f8838707388&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47");
        }

        private void TriageClick(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("• Appropriate Technical Resources Engaged (Y/N): ");
            sb.AppendLine("• Current Customer Impact: \n\n" + BI + "\n\n");
            sb.AppendLine("• Customer Temp: ");
            sb.AppendLine("• Current Status & Action Plan: \n\n" + CS + "\n\n" + AP);
            sb.AppendLine("• Roadblocks (Y/N): N ");
            sb.AppendLine("• PFE Discussed (Y/N): N ");
            sb.AppendLine("• Should case be Escalated to CMET (Y/N): N ");
            sb.AppendLine("• Participants: ");
            sb.AppendLine("• Questions/Concerns: ");
            sb.AppendLine("• Outcome of the triage: ");
            Clipboard.SetText(sb.ToString());

            MessageBox.Show("Clipboard contents set to:\n\n" + Clipboard.GetText(), "Clipboard", MessageBoxButton.OK, MessageBoxImage.None);
        }
    }
}
