using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WpfApp1;

/* This application is programmed by Dustin Conlon, employed with Tek Experts 
 * 
 * This was made as an assistant tool for managing CritSit (Severity A) Premier Support Cases
 * data is stored locally for THIS instance of the program, closing the software deletes the ENTIRE directory where data is saved
 * This application does NOT forward information to an outside location for storage, please view the code for proof
 * Additionally, this data does NOT directly pull information from the Dynamics portal, nor does it manipulate it in any way
 * 
 * Please ping or email v-duconl for additional information or questions 
 * Source code available on request */

namespace CMET_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }

        DateTime[] sixHours;
        List<Reminder> ReminderList = new List<Reminder>();
        int ReminderIndex = 0;

        public MainWindow()
        {
            Directory.CreateDirectory(Environment.SpecialFolder.ApplicationData + "//.cmetDataAssist");
            Thread t = new Thread(new ThreadStart(ReminderThread));
            ReminderList.Clear();
            InitializeComponent();
        }

        public void ReminderThread()
        {
            while (ReminderList.Count > 0)
            {
                foreach (Reminder entry in ReminderList)
                {
                    if (DateTime.Now > entry.ReminderTime)
                    {

                    }
                }
            }
        }

        #region Browser Controls and Functions
        /* This section is dedicated to the built-in webbrowser
         * It does not store information, it navigates to any Address inserted in the Address box,
         * or to any "bookmarks" predefined in the code */

        // This minimizes the browser
        //private void BrowserMinimize(object sender, RoutedEventArgs e)
        //{
        //    if ((sender as CheckBox).IsChecked == false)
        //    {
        //        this.Height = 340;
        //        URLBox.IsEnabled = false;
        //        NavButton.IsEnabled = false;
        //        RefreshButton.IsEnabled = false;
        //        SDButton.IsEnabled = false;
        //        RaveButton.IsEnabled = false;
        //        RadiusButton.IsEnabled = false;
        //        PortalButton.IsEnabled = false;
        //        TeamsButton.IsEnabled = false;
        //    }
        //    else
        //    {
        //        this.Height = 1080;
        //        URLBox.IsEnabled = true;
        //        NavButton.IsEnabled = true;
        //        RefreshButton.IsEnabled = true;
        //        SDButton.IsEnabled = true;
        //        RaveButton.IsEnabled = true;
        //        RadiusButton.IsEnabled = true;
        //        PortalButton.IsEnabled = true;
        //        TeamsButton.IsEnabled = true;
        //    }
        //}

        //// This navigates the browser to the Address in the box
        //private void Navigate(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = URLBox.Text;
        //}

        //// This refreshes the page on the browser
        //private void Refresh(object sender, RoutedEventArgs e)
        //{
            
        //}

        //// This auto-navigates the browser to MS Service Desk
        //private void NavServiceDesk(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = "https://servicedesk.microsoft.com/#/home";
        //}

        //// This auto-navigates the browser to RAVE
        //private void NavRave(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = "https://rave.office.net/";
        //}

        //// This auto-navigates the browser to Radius
        //private void NavRadius(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = "https://expert.partners.extranet.microsoft.com/expert/Radius?";
        //}

        //// This auto-navigates the browser to the Dynamics 365 Crit Portal
        //private void NavPortal(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = "https://critsit.crm.dynamics.com/main.aspx#792162642";
        //}

        //// This auto-navigates the browser to Teams RI Board
        //private void NavTeams(object sender, RoutedEventArgs e)
        //{
        //    Browser.Address = "https://teams.microsoft.com/l/entity/com.microsoft.teamspace.tab.planner/_djb2_msteams_prefix_2142145758?webAddress=https%3a%2f%2ftasks.office.com%2fmicrosoft.onmicrosoft.com%2fHome%2fPlanViews%2f5DE9a5GowE-r9pk7C5GLXpUADHTF&label=Escalations%2fResource+Issues&context=%7b%0d%0a++%22subEntityId%22%3a+null%2c%0d%0a++%22canvasAddress%22%3a+%22https%3a%2f%2ftasks.office.com%2fmicrosoft.onmicrosoft.com%2fHome%2fPlannerFrame%3fpage%3d7%26planId%3d5DE9a5GowE-r9pk7C5GLXpUADHTF%26auth_pvr%3dOrgid%26auth_upn%3d%7bupn%7d%26mkt%3d%7blocale%7d%22%2c%0d%0a++%22channelId%22%3a+%2219%3ac6b9db685f4c4a21b7a0cd874d574723%40thread.skype%22%0d%0a%7d&groupId=ff4a4b74-a580-4cb9-b47d-7f8838707388&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47";
        //}

        #endregion

        #region SR Manipulation
        // This adds the current case to the ListBox provided
        private void AddSRButton(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SRBox.Text) && !string.IsNullOrWhiteSpace(BusNameBox.Text))
            {
                string ToAdd = SRBox.Text.Substring(8) + " - " + BusNameBox.Text.Substring(0, 8);
                SRList.Items.Add(ToAdd);
            }
        }

        // This removes the selected SR from the SRList ListBox
        private void RemoveSRButton(object sender, RoutedEventArgs e)
        {
            SRList.Items.RemoveAt(SRList.SelectedIndex);
            // REMOVE SAVED DATA FOR REMOVED SR CASE
        }

        // This occurs when user selects an SR from the SRList ListBox
        private void SRListIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // CODE TO TEST IF FILE EXISTS
            if (File.Exists(Environment.SpecialFolder.ApplicationData + "//.cmetDataAssist//" + SRList.SelectedItem.ToString()))
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            // CODE TO LOAD DATA FROM FILE
            string[] data = File.ReadAllLines(Environment.SpecialFolder.ApplicationData + "//.cmetDataAssist//" + SRList.SelectedItem.ToString());
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < data.Length; index++)
            {
                if (data[index] == "Begin Overview:")
                {
                    while (data[index] != "End Overview")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO OVBox
                    sb.Clear();
                }
                if (data[index] == "Begin Business Impact:")
                {
                    while (data[index] != "End Business Impact")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO BIBox
                    sb.Clear();
                }
                if (data[index] == "Begin Current Status:")
                {
                    while (data[index] != "End Current Status")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO CSBox
                    sb.Clear();
                }
                if (data[index] == "Begin Action Plan MS:")
                {
                    sb.AppendLine("Microsoft: ");
                    while (data[index] != "End Action Plan MS")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO APMSBox
                    sb.Clear();
                }
                if (data[index] == "Begin Action Plan Bus:")
                {
                    while (data[index] != "End Action Plan Bus")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO APBUSBox
                    sb.Clear();
                }
                if (data[index] == "Begin Internal:")
                {
                    while (data[index] != "End Internal")
                    {
                        sb.AppendLine(data[index]);
                        index++;
                    }
                    // SEND SB DATA TO APBUSBox
                    sb.Clear();
                }
            }
        }
        #endregion

        // This occurs when the user closes the application, it closes all child windows, and deletes ALL data in the /AppData/Roaming/.cmetDataAssist/ folder
        private void ClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Closing the program will delete ALL data, are you sure?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Directory.Delete(Environment.SpecialFolder.ApplicationData + "//.cmetDataAssist");
                //Cef.Shutdown();
            }
            else
               e.Cancel = true;
        }

        // This function is called when any data-priority TextBoxes have their text changed - it calles the SaveData function
        private void TextChangedSave(object sender, TextChangedEventArgs e)
        {
            SaveData();
        }

        // This function is called when any data-priority CheckBoxes have been checked/unchecked - it calles the SaveData function
        private void CheckBoxChangedSave(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        // This function is called when any data-priority RadioButtons have been checked/unchecked - it calles the SaveData function
        private void RadioButtonChangedSave(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {

        }

        #region Email Functions
        // These functions auto-generate emails that go to their respective recipients
        // They do NOT add a signature on purpose, that step must be done by the CSM to assist with preventing PII issues
        // TODO: Implement .xml template model

        // This auto-generates an email with standard Business Impact questions
        private void BIEmailClick(object sender, RoutedEventArgs e)
        {
            Outlook.Application app = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.Subject = "Current Status - " + Regex.Replace(SRBox.Text, @"\s+", "") + " - " + BusNameBox.Text;
            mailItem.To = CxEmailBox.Text;
            mailItem.CC = "crit365; casemail; " + TAMAliasBox.Text + "; " + BTAMAliasBox.Text + "; ";
            mailItem.Body = CxNameBox.Text.Split(' ')[0] + ",\n\nI understand you are very busy, but I would like to ask a few questions to ensure your case is handled properly and esclated correctly as necessary.\n" +
                "When you have a few free minutes, please reply with answers to the following questions:\n\nHow is this issue affecting your Business?\nIs this impacting a production environment, or a development environment?\n" +
                "How many users are affected by this issue?\nWhat troubleshooting steps have you taken already, in an attempt to mitigate the issue?\n\nThank you,\n";
            mailItem.Display(true);
        }

        private void TriageEmailButton(object sender, RoutedEventArgs e)
        {
            Outlook.Application app = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.Subject = "Current Status - " + Regex.Replace(SRBox.Text, @"\s+", "") + " - " + BusNameBox.Text;
            mailItem.To = CxEmailBox.Text;
            mailItem.CC = "crit365; casemail; " + TAMAliasBox.Text + "; " + BTAMAliasBox.Text + "; " + EngiAliasBox.Text;
            mailItem.Body = CxNameBox.Text.Split(' ')[0] + ",\n\nI understand you are very busy, but this case has been alive for about 6 hours and since I am unable to contact you by phone, could you please answer the following questions " +
                "when you have a few free minutes:\n\nDo you feel that the current Microsoft Engineer is the correct person to be able to resolve the issue?\nHas the impact to your business changed (i.e. affecting more users/more departments) " +
                "from what was mentioned previously?\nDo you have any other administrative-oriented questions or concerns related to this case that I can answer?\n\nThank you,\n";
            mailItem.Display(true);
        }

        private void LoweringEmailClick(object sender, RoutedEventArgs e)
        {
            Outlook.Application app = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.Subject = "Current Status - " + Regex.Replace(SRBox.Text, @"\s+", "") + " - " + BusNameBox.Text;
            mailItem.To = CxEmailBox.Text;
            mailItem.CC = "crit365; casemail; " + TAMAliasBox.Text + "; " + BTAMAliasBox.Text + "; " + EngiAliasBox.Text;
            mailItem.Body = CxNameBox.Text.Split(' ')[0] + ",\n\nThank you for your time on our call, with the permission that you have given Microsoft, we will proceed to lower the severity of the case. Should you " +
                "need to re-raise the severity to Critical (Severity A) please call your region's Premier Hotline number and reference the case number in the subject heading of all of our emails.\n\nYou should recieve an official email " +
                "regarding this change in severity shortly.\n\nThank you,\n";
            mailItem.Display(true);
        }

        private void ArchiveEmailClick(object sender, RoutedEventArgs e)
        {
            Outlook.Application app = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.Subject = "Current Status - " + Regex.Replace(SRBox.Text, @"\s+", "") + " - " + BusNameBox.Text;
            mailItem.To = CxEmailBox.Text;
            mailItem.CC = "crit365; casemail; " + TAMAliasBox.Text + "; " + BTAMAliasBox.Text + "; " + EngiAliasBox.Text;
            mailItem.Body = CxNameBox.Text.Split(' ')[0] + ",\n\nThank you for your time on our call, with the permission that you have given Microsoft, we will proceed to archive the case. Should you " +
                "need to re-open the case, please call your region's Premier Hotline number and reference the case number in the subject heading of all of our emails.\n\nYou should recieve an official email " +
                "regarding this change shortly.\n\nThank you,\n";
            mailItem.Display(true);
        }
        #endregion

        private void ReminderSetClick(object sender, RoutedEventArgs e)
        {
            ReminderList.Add(new Reminder() { ReminderText = ReminderBox.Text, ReminderTime = (DateTime)ReminderTime.Value });
            ReminderIndex++;
            ReminderView.ItemsSource = null;
            ReminderView.ItemsSource = ReminderList;
        }

        private void ReminderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ReminderListDelete(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("KEY DOWN " + ReminderView.SelectedIndex);
            if (ReminderView.SelectedIndex > -1 && ReminderView.SelectedIndex <= ReminderView.Items.Count)
            {
                if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
                {
                    Debug.WriteLine("REMOVE HERE");
                    ReminderList.Remove((Reminder)ReminderView.SelectedItem);
                    ReminderView.ItemsSource = null;
                    ReminderView.ItemsSource = ReminderList;
                }
            }
        }
    }
}