using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WpfApp1;
using System.Xml;
using System.Threading.Tasks;
using System.IO;
using Case_Assistant;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

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
            int QueryService( [In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject );
        }

        XmlDocument DataHandler = new XmlDocument();
        DateTime[] sixHours;
        public Case CurrentCase { get; set; }
        public ObservableCollection<Reminder> _Reminders = new ObservableCollection<Reminder>();
        public ObservableCollection<Reminder> Reminders { get { return _Reminders; } }
        public ObservableCollection<Case> _Cases = new ObservableCollection<Case>();
        public ObservableCollection<Case> Cases { get { return _Cases; } }
        CaseStarter _CaseStarter;
        LaborControl laborCont;
        CreateDataWindow dataWindow;
        Thread ReminderThread;

        EmailFunctions emailFunctions = new EmailFunctions();

        public MainWindow()
        {
            _Reminders.Clear();
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            ReminderTime.Step = 10;
            ReminderTime.TimeInterval = new TimeSpan(0, 10, 0);
            ReminderThread = new Thread(new ThreadStart(ThreadProcess));
            laborCont = new LaborControl(this);
            ContentBox.Content = laborCont;
            ReminderThread.IsBackground = true;
            ReminderThread.Start();
            ReminderView.DataContext = this;
            dataWindow = new CreateDataWindow(this);
            MediaPlayer.LoadedBehavior = MediaState.Manual;
            MediaPlayer.UnloadedBehavior = MediaState.Manual;
            DisableControls();
        }

        private void DisableControls()
        {
            SRBox.IsEnabled = false;
            BusNameBox.IsEnabled = false;
            TAMBox.IsEnabled = false;
            TAMAliasBox.IsEnabled = false;
            BTAMBox.IsEnabled = false;
            BTAMAliasBox.IsEnabled = false;
            EngiBox.IsEnabled = false;
            EngiAliasBox.IsEnabled = false;
            TechBox.IsEnabled = false;
            DMBox.IsEnabled = false;
            RaveNumBox.IsEnabled = false;
            IcMBox.IsEnabled = false;
            LinkBox.IsEnabled = false;
            BridgeLinkBox.IsEnabled = false;
            PolCheck.IsEnabled = false;
            SecCheck.IsEnabled = false;
            CusCheck.IsEnabled = false;
            PGCheck.IsEnabled = false;
            TAMCheck.IsEnabled = false;
            DataCompCheck.IsEnabled = false;
            EngiEngagedCheck.IsEnabled = false;
            CaseActiveCheck.IsEnabled = false;
            CxNameBox.IsEnabled = false;
            CxPhoneBox.IsEnabled = false;
            CxEmailBox.IsEnabled = false;
            ContactsBox.IsEnabled = false;
            PhoneRadio.IsEnabled = false;
            EmailRadio.IsEnabled = false;
            SkypeRadio.IsEnabled = false;
            WebExRadio.IsEnabled = false;
            OVBox.IsEnabled = false;
            BIBox.IsEnabled = false;
            CSBox.IsEnabled = false;
            MSAPBox.IsEnabled = false;
            BusAPBox.IsEnabled = false;
            InternalBox.IsEnabled = false;
            PersonalBox.IsEnabled = false;

            foreach (FrameworkElement element in MadeContactGrid.Children)
            {
                if (element is Button)
                    (element as Button).IsEnabled = false;
            }

            ContentBox.Content = null;
        }

        private void EnableControls()
        {
            SRBox.IsEnabled = true;
            BusNameBox.IsEnabled = true;
            TAMBox.IsEnabled = true;
            TAMAliasBox.IsEnabled = true;
            BTAMBox.IsEnabled = true;
            BTAMAliasBox.IsEnabled = true;
            EngiBox.IsEnabled = true;
            EngiAliasBox.IsEnabled = true;
            TechBox.IsEnabled = true;
            DMBox.IsEnabled = true;
            RaveNumBox.IsEnabled = true;
            IcMBox.IsEnabled = true;
            LinkBox.IsEnabled = true;
            BridgeLinkBox.IsEnabled = true;
            PolCheck.IsEnabled = true;
            SecCheck.IsEnabled = true;
            CusCheck.IsEnabled = true;
            PGCheck.IsEnabled = true;
            TAMCheck.IsEnabled = true;
            DataCompCheck.IsEnabled = true;
            EngiEngagedCheck.IsEnabled = true;
            CaseActiveCheck.IsEnabled = true;
            CxNameBox.IsEnabled = true;
            CxPhoneBox.IsEnabled = true;
            CxEmailBox.IsEnabled = true;
            ContactsBox.IsEnabled = true;
            PhoneRadio.IsEnabled = true;
            EmailRadio.IsEnabled = true;
            SkypeRadio.IsEnabled = true;
            WebExRadio.IsEnabled = true;
            OVBox.IsEnabled = true;
            BIBox.IsEnabled = true;
            CSBox.IsEnabled = true;
            MSAPBox.IsEnabled = true;
            BusAPBox.IsEnabled = true;
            InternalBox.IsEnabled = true;
            PersonalBox.IsEnabled = true;

            foreach (FrameworkElement element in MadeContactGrid.Children)
            {
                if (element is Button)
                    (element as Button).IsEnabled = true;
            }

            ContentBox.Content = laborCont;
        }

        private void MainWindow_Closing( object sender, CancelEventArgs e )
        {
            MessageBoxResult result = MessageBox.Show("Closing the program will delete all the data you have entered.\n\nIf you no longer need this software, feel free to close it. Otherwise, it is recommended" +
                "to minimize the application until further use is necessary.\n\nPress OK to close the software, otherwise press Cancel", "Are you sure you wish to close?", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
            else
            {
                ReminderThread.Abort();
                e.Cancel = false;
                System.Environment.Exit(1);
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
        // This removes the selected SR from the SRList ListBox
        private void RemoveSRButton( object sender, RoutedEventArgs e )
        {
            if (SRList.SelectedIndex > -1 && SRList.SelectedItem != null)
            {
                int index = SRList.SelectedIndex;
                Cases.RemoveAt(index);
                SRList.SelectedIndex = Cases.Count - 1;
            }
        }

        // This occurs when user selects an SR from the SRList ListBox
        private void SRListIndexChanged( object sender, SelectionChangedEventArgs e )
        {
            if (SRList.SelectedIndex > -1)
            {
                SRGrid.DataContext = SRList.SelectedItem;
                SRBox.DataContext = SRList.SelectedItem;
                BusNameBox.DataContext = SRList.SelectedItem;
                ContactGrid.DataContext = SRList.SelectedItem;
                AthenaNotes.DataContext = SRList.SelectedItem;
                CommView.DataContext = SRList.SelectedItem;
                laborCont.ChangeDataContext();
                (SRList.SelectedItem as Case).SixHour = DateTime.Now.AddHours(5);
                SixHourPicker.DataContext = SRList.SelectedItem;
                EnableControls();
            }
            else
            {
                SRBox.Text = "";
                BusNameBox.Text = "";
                TAMBox.Text = "";
                TAMAliasBox.Text = "";
                BTAMBox.Text = "";
                BTAMAliasBox.Text = "";
                EngiBox.Text = "";
                EngiAliasBox.Text = "";
                DMBox.Text = "";
                RaveNumBox.Text = "";
                IcMBox.Text = "";
                LinkBox.Text = "";
                PolCheck.IsEnabled = false;
                SecCheck.IsEnabled = false;
                CusCheck.IsEnabled = false;
                PGCheck.IsEnabled = false;
                TAMCheck.IsEnabled = false;
                DataCompCheck.IsEnabled = false;
                CxNameBox.Text = "";
                CxPhoneBox.Text = "";
                CxEmailBox.Text = "";
                ContactsBox.Text = "";
                PhoneRadio.IsEnabled = false;
                EmailRadio.IsEnabled = false;
                SkypeRadio.IsEnabled = false;
                WebExRadio.IsEnabled = false;
                OVBox.Text = "";
                BIBox.Text = "";
                CSBox.Text = "";
                MSAPBox.Text = "";
                BusAPBox.Text = "";
                InternalBox.Text = "";
                PersonalBox.Text = "";
            }
        }

        // This opens a Create Case dialog to allow users to add a case to the application
        private void CreateCaseButton( object sender, RoutedEventArgs e )
        {
            _CaseStarter = new CaseStarter(this);
            _CaseStarter.Left = this.Left + 50;
            _CaseStarter.Top = this.Top + 50;
            _CaseStarter.Show();
        }

        // This is called from the Case Creator dialog, it adds the case to the ObservableCollection
        public void CreateCase( string SRNumber, string BusinessName )
        {
            _Cases.Add(new Case() { SRNum = SRNumber, BusName = BusinessName });
            _CaseStarter.Close();
            SRList.SelectedIndex = Cases.Count - 1;
        }
        #endregion

        #region Email Functions
        // These functions auto-generate emails that go to their respective recipients
        // They do NOT add a signature on purpose, that step must be done by the CSM to assist with preventing PII issues
        // TODO: Implement .xml template model

        private void BIEmailClick( object sender, RoutedEventArgs e )
        {
            emailFunctions.BIEmailTemplate(CxNameBox.Text);
        }

        private void TriageEmailButton( object sender, RoutedEventArgs e )
        {
            emailFunctions.TriageEmailTemplate(CxNameBox.Text);
        }

        private void LoweringEmailClick( object sender, RoutedEventArgs e )
        {
            emailFunctions.LoweringEmailTemplate(CxNameBox.Text);
        }

        private void ArchiveEmailClick( object sender, RoutedEventArgs e )
        {
            emailFunctions.ArchivingEmailTemplate(CxNameBox.Text);
        }

        private void RIClick( object sender, RoutedEventArgs e )
        {
            emailFunctions.RIEmailGenerate(SRList.SelectedItem as Case);
        }

        private void SetRulesClick( object sender, RoutedEventArgs e )
        {
            MessageBoxResult result = MessageBox.Show("Please ensure you already have a rule that contains \"Cases\" in the name of the rule and that the folder you want it to move emails to is already defined\n\n" +
                "Press Yes if you already have done this, otherwise, press No and setup the rule, then comeback and press the \"Set Email Rules\" button again.", "Already setup?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                List<string> caseNumberList = new List<string>();

                for (int i = 0; i < SRList.Items.Count; i++)
                {
                    caseNumberList.Add((SRList.Items[i] as Case).SRNum);
                    Debug.WriteLine(caseNumberList[i]);
                }

                emailFunctions.SetEmailRules(caseNumberList.Cast<object>().ToArray());
            }
        }
        #endregion

        #region Reminder Functions
        private void ReminderSetClick( object sender, RoutedEventArgs e )
        {
            SetReminder();
        }

        private void ReminderBoxKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                SetReminder();
        }

        private void ReminderHelpClick( object sender, RoutedEventArgs e )
        {
            MessageBox.Show("Fill in the time you would like to be reminded OR how long, in minutes, you want the reminder to appear, then press Set.\n\nTo remove a reminder before it has gone off, select the reminder and" +
                "press the Delete or Backspace key on your keyboard", "Reminder help", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void SetReminder()
        {
            Reminder r = null;
            if (ReminderTime.Value != null && String.IsNullOrWhiteSpace(ReminderMin.Text))
            {
                r = new Reminder() { ReminderText = ReminderBox.Text, ReminderTime = (DateTime) ReminderTime.Value };
                goto ContinueStatement;
            }
            else if (!String.IsNullOrWhiteSpace(ReminderMin.Text) && ReminderTime.Value == null)
            {
                Double time = 0;
                double.TryParse(Regex.Replace(ReminderMin.Text, "[^0-9.]", ""), out time);
                if (time != 0)
                    r = new Reminder() { ReminderText = ReminderBox.Text, ReminderTime = DateTime.Now.AddMinutes(Double.Parse(Regex.Replace(ReminderMin.Text, "[^0-9.]", ""))) };
                else
                    MessageBox.Show("Time value must equal more than 0 or null");
                goto ContinueStatement;
            }
            else
            {
                MessageBox.Show("Error: Please enter a time to be reminded, or how long from now (in minutes) you want the reminder to occur", "Error", MessageBoxButton.OK, MessageBoxImage.None);
                goto EndStatement;
            }
        ContinueStatement:
            ReminderBox.Text = "";
            _Reminders.Add(r);
            ReminderTime.Value = null;
            ReminderMin.Text = "";
        EndStatement:
            Debug.WriteLine("Ending Statement");
        }

        private void ReminderListDelete( object sender, System.Windows.Input.KeyEventArgs e )
        {
            Debug.WriteLine("KEY DOWN " + ReminderView.SelectedIndex);
            if (ReminderView.SelectedIndex > -1 && ReminderView.SelectedIndex <= ReminderView.Items.Count)
            {
                if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
                {
                    Debug.WriteLine("REMOVE HERE");
                    _Reminders.Remove((Reminder) ReminderView.SelectedItem);
                    Thread.Sleep(50);
                }
            }
        }

        public void ThreadProcess()
        {
            while (true)
            {
                foreach (Reminder entry in _Reminders.ToList())
                {
                    if (DateTime.Now.CompareTo(entry.ReminderTime) > 0)
                    {
                        Application.Current.Dispatcher.Invoke((Action) (() =>
                         {
                             MediaPlayer.Source = new Uri(@"C:\\Windows\\media\\Alarm10.wav");
                             MediaPlayer.Position = new TimeSpan(0);
                             MediaPlayer.Play();
                             MessageBox.Show("You have a reminder set:\n\n" + entry.ReminderText, "Reminder", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                             _Reminders.Remove(entry);
                         }));
                    }
                }

                //foreach (Case c in Cases)
                //{
                //    if (DateTime.Now.CompareTo(c.SixHour) > 0)
                //    {
                //        MessageBox.Show("You have 6-hour Athena Tasks due for SR#" + c.SRNum + "\n\n" + c.BusName, "Six-Hour tasks due", MessageBoxButton.OK, MessageBoxImage.Warning);
                //        Application.Current.Dispatcher.Invoke((Action)(() =>
                //        {
                //            c.SixHour = DateTime.Now.AddHours(6);
                //            Thread.Sleep(200);
                //            SixHourPicker.DataContext = null;
                //            Thread.Sleep(200);
                //            SixHourPicker.DataContext = this;
                //        }));
                //    }
                //}
                Thread.Sleep(500);
            }
        }
        #endregion

        #region Athena Notes Functions
        private void OVCopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            Clipboard.SetText(OVBox.Text);
        }

        private void BICopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            Clipboard.SetText(BIBox.Text);
        }

        private void CSCopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            Clipboard.SetText(CSBox.Text);
        }

        private void APCopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            StringBuilder mssb = new StringBuilder();
            string[] MSlines = MSAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in MSlines)
                mssb.AppendLine("- " + s);

            StringBuilder bussb = new StringBuilder();
            string[] Buslines = BusAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in Buslines)
                bussb.AppendLine("- " + s);

            Clipboard.SetText("Microsoft:\n" + mssb.ToString() + "\n" + BusNameBox.Text + ":\n" + bussb.ToString());
        }

        private void InternalCopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            Clipboard.SetText(InternalBox.Text);
        }

        private void OVCopyClick( object sender, RoutedEventArgs e )
        {
            Clipboard.SetText(OVBox.Text);
        }

        private void BICopyClick( object sender, RoutedEventArgs e )
        {
            Clipboard.SetText(BIBox.Text);
        }

        private void CSCopyClick( object sender, RoutedEventArgs e )
        {
            Clipboard.SetText(CSBox.Text);
        }

        private void APCopyClick( object sender, RoutedEventArgs e )
        {
            StringBuilder mssb = new StringBuilder();
            string[] MSlines = MSAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in MSlines)
                mssb.AppendLine("- " + s);

            StringBuilder bussb = new StringBuilder();
            string[] Buslines = BusAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in Buslines)
                bussb.AppendLine("- " + s);

            Clipboard.SetText("Microsoft:\n" + mssb.ToString() + "\n" + BusNameBox.Text + ":\n" + bussb.ToString());
        }

        private void InternalCopyClick( object sender, RoutedEventArgs e )
        {
            Clipboard.SetText(InternalBox.Text);
        }

        private void PersonalCopyClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            Clipboard.SetText(PersonalBox.Text);
        }

        private void PersonalCopyClick( object sender, RoutedEventArgs e )
        {
            Clipboard.SetText(PersonalBox.Text);
        }


        #endregion

        #region Communication Buttons and Functions
        // Runs when "Made Call" is clicked
        private void CallClick( object sender, RoutedEventArgs e )
        {
            CallPanel _CallPanel = new CallPanel(CxNameBox.Text.Split(' ')[0], this);
            ContentBox.Content = _CallPanel;
        }
        // Runs when "Sent Email" is clicked
        private void EmailClick( object sender, RoutedEventArgs e )
        {
            EmailPanel _EmailPanel = new EmailPanel(CxNameBox.Text.Split(' ')[0], this);
            ContentBox.Content = _EmailPanel;
        }
        // Runs when "Joined WebEx" is clicked
        private void WebExClick( object sender, RoutedEventArgs e )
        {
            (SRList.SelectedItem as Case)._Comms.Add(new Communication()
            {
                Name = "WebEx",
                Time = DateTime.UtcNow,
                Type = Communication._Type.WebEx,
                FullContent = "WebEx\t----\t" +
                DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + " UTC" + "\tJoined WebEx conference",
                Content = "Joined WebEx Conference"
            });
        }
        // Runs when "Skype Convo" is clicked
        private void SkypeClick( object sender, RoutedEventArgs e )
        {
            SkypePanel _SkypePanel = new SkypePanel(this);
            ContentBox.Content = _SkypePanel;
        }
        // Runs when "Joined Bridge" is clicked
        private void BridgeClick( object sender, RoutedEventArgs e )
        {
            (SRList.SelectedItem as Case)._Comms.Add(new Communication()
            {
                Name = "Bridge",
                Time = DateTime.UtcNow,
                Type = Communication._Type.Bridge,
                FullContent = "Bridge\t----\t" +
                DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + " UTC" + "\tJoined phone bridge call",
                Content = "Joined Bridge Call"
            });
        }
        // Runs when Call Dialog is closed
        public void CallComplete( string To_Internal = "", string content = "", string description = "" )
        {
            (SRList.SelectedItem as Case)._Comms.Add(new Communication() { Name = CxNameBox.Text.Split(' ')[0], Time = DateTime.UtcNow, Type = Communication._Type.Call, FullContent = To_Internal, Content = content, Description = description });
            ContentBox.Content = laborCont;
        }
        // Runs when Email Dialog is closed
        public void EmailSent( string To_Internal = "", string content = "", string description = "" )
        {
            (SRList.SelectedItem as Case)._Comms.Add(new Communication() { Name = CxNameBox.Text.Split(' ')[0], Time = DateTime.UtcNow, Type = Communication._Type.Email, FullContent = To_Internal, Content = content, Description = description });
            ContentBox.Content = laborCont;
        }
        // Runs when Skype Dialog is closed
        public void SkypeConvo( string with = "", string To_Internal = "", string content = "", string description = "" )
        {
            (SRList.SelectedItem as Case)._Comms.Add(new Communication() { Name = with, Time = DateTime.UtcNow, Type = Communication._Type.Skype, FullContent = To_Internal, Content = content, Description = To_Internal.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0] });
            ContentBox.Content = laborCont;
        }
        // Copies all non-skype elements in Comms ObservableCollection
        private void CopyCommClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            foreach (Communication comm in (SRList.SelectedItem as Case).Comms)
            {
                if (comm.Type != Communication._Type.Skype)
                {
                    sb.AppendLine(comm.FullContent);
                }
            }
            Clipboard.SetText(sb.ToString());
        }
        // Copies all skype elements in Comms ObservableCollection
        private void CopySkypeClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            foreach (Communication comm in (SRList.SelectedItem as Case).Comms)
            {
                if (comm.Type == Communication._Type.Skype)
                {
                    sb.AppendLine(comm.FullContent);
                    sb.Append("\n\n");
                }
            }
            Clipboard.SetText(sb.ToString());
        }
        // Copies all elements in Comms ObservableCollection
        private void CopyAllCommClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            foreach (Communication comm in (SRList.SelectedItem as Case).Comms)
            {
                sb.AppendLine(comm.FullContent);
            }
            Clipboard.SetText(sb.ToString());
        }
        #endregion

        #region Skype "Chat" Button Functions
        // Opens Lync chat for TAM Alias in TAMAliasBox
        private void TamChatClick( object sender, RoutedEventArgs e )
        {
            Process.Start("sip:" + TAMAliasBox.Text + "@microsoft.com");
        }
        // Opens Lync chat for BTAM Alias in BTAMAliasBox
        private void BTamChatClick( object sender, RoutedEventArgs e )
        {
            Process.Start("sip:" + BTAMAliasBox.Text + "@microsoft.com");
        }
        // Opens Lync chat for Engineer Alias in EngiAliasBox
        private void EngiChatClick( object sender, RoutedEventArgs e )
        {
            Process.Start("sip:" + EngiAliasBox.Text + "@microsoft.com");
        }
        #endregion

        private void OpenSRClick( object sender, RoutedEventArgs e )
        {
            Process.Start(LinkBox.Text);
        }



        private void RITemplateClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Resource Issue on SR " + SRBox.Text + " - Engineer Wait Time: \n");
            sb.Append("Technology : " + TechBox.Text + "\n");
            sb.Append("Time CritSit was first opened or reopened : \n");
            sb.Append("Time CritSit was dispatched to queue at A : \n");
            sb.Append("DM Handling Case :  (Alias) " + DMBox.Text + "\n");
            sb.Append("Is SEM / M1 Engaged : " + M1Check.IsChecked + "\n");
            sb.Append("Is M2 Engaged : " + M2Check.IsChecked + "\n");
            sb.Append("Time the first DM chasing email was sent : " + (SRList.SelectedItem as Case).DMEmailSent.ToString("HH:mm") + " UTC\n");
            sb.Append("Time the first M1/M2 chasing email was sent : \n");
            sb.Append("Time the second M1/M2 chasing email was sent : \n");
            sb.Append("Is the Cluster Lead aware (AL) : \n");
            sb.Append("Has a Process Breakdown been submitted : \n");
            sb.Append("Customer temperature : \n");
            Clipboard.SetText(sb.ToString());
        }

        private void IcMCopyClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-=-=-=- IcM Number: " + IcMBox.Text + " -=-=-=-");
        }

        private void TestDMList( object sender, TextChangedEventArgs e )
        {
            //string[] data = File.ReadAllLines("C:\\Users\\" + Environment.UserName + "\\Documents\\DMAliases.csv");
            //foreach (string s in data)
            //{
            //    if (TechBox.Text == s.Split(',')[0])
            //        DMBox.Text = s.Split(',')[1];
            //    else
            //        DMBox.Text = "";
            //}

            if (TechBox.Text.Contains("Dynamics"))
                DMBox.Text = "dyndm";
            else if (TechBox.Text.Contains("Skype") || TechBox.Text.Contains("Lync"))
                DMBox.Text = "uclyncdm";
            else if (TechBox.Text.Contains("Azure"))
            {
                if (TechBox.Text.Contains("Kubernetes") || TechBox.Text.Contains("Virtual Networks"))
                    DMBox.Text = "platgldm; wadm;";
                else if (TechBox.Text.Contains("Windows"))
                    DMBox.Text = "platgldm";
                else if (TechBox.Text.Contains("Cosmos") || TechBox.Text.Contains("DB") || TechBox.Text.Contains("SQL"))
                    DMBox.Text = "sqldm";
                else if (TechBox.Text.Contains("HDInsight"))
                    DMBox.Text = "sqldm; hadoopsupport;";
                else if (TechBox.Text.Contains("AD"))
                    DMBox.Text = "platgldm";
                else if (TechBox.Text.Contains("StorSimple"))
                    DMBox.Text = "SSSuptteam";
                else
                    DMBox.Text = "aztechim";
            }
            else if (TechBox.Text.Contains("Exchange"))
            {
                if (TechBox.Text.Contains("Online"))
                    DMBox.Text = "exchonlinedm";
                else if (TechBox.Text.Contains("Server"))
                    DMBox.Text = "exchonpremdm";
            }
            else if (TechBox.Text.Contains("Outlook"))
                DMBox.Text = "exchonlinedm";
            else if (TechBox.Text.Contains("SQL"))
            {
                if (TechBox.Text.Contains("Svr 2014 Enterprise"))
                    DMBox.Text = "platgldm; wadm;";
                else
                    DMBox.Text = "sqldm; platgldm;";
            }
            else if (TechBox.Text.Contains("Sys"))
                DMBox.Text = "scsdm";
            else if (TechBox.Text.Contains("Windows") || TechBox.Text.Contains("WSUS"))
                DMBox.Text = "platgldm";
            else if (TechBox.Text.Contains("Sharepoint") || TechBox.Text.Contains("Office 365") || TechBox.Text.Contains("OneDrive"))
                DMBox.Text = "offdm";
            else if (TechBox.Text.Contains("Microsoft Intune"))
                DMBox.Text = "mobsuppodww";
            else if (TechBox.Text.Contains("Identity"))
                DMBox.Text = "secusdm";
            else if (TechBox.Text.Contains("Project Server"))
                DMBox.Text = "offdm";
            else
                DMBox.Text = "NOT FOUND - PING DUSTIN";
        }

        private void RaveClick( object sender, RoutedEventArgs e )
        {
            Process.Start("https://rave.office.net/search?query=" + RaveNumBox.Text);
        }

        private void LinksButtonClick( object sender, RoutedEventArgs e )
        {
            StringBuilder mssb = new StringBuilder();
            string[] MSlines = MSAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in MSlines)
                mssb.AppendLine("- " + s);

            StringBuilder bussb = new StringBuilder();
            string[] Buslines = BusAPBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in Buslines)
                bussb.AppendLine("- " + s);
            string AP = "Microsoft:\n" + mssb.ToString() + "\n" + BusNameBox.Text + ":\n" + bussb.ToString();
            Links _Links = new Links(SRBox.Text, BIBox.Text, CSBox.Text, AP, BusNameBox.Text);
            ContentBox.Content = _Links;
        }

        private void LaborButtonClick( object sender, RoutedEventArgs e )
        {
            ContentBox.Content = laborCont;
        }

        private void CreateDataClick( object sender, RoutedEventArgs e )
        {
            dataWindow.Show();
        }

        public void CloseDataWindow()
        {
            dataWindow.DataBox.Text = "";
            dataWindow.Close();
        }

        private void OpenBridgeClick( object sender, RoutedEventArgs e )
        {
            Process.Start(BridgeLinkBox.Text);
        }

        //  Alt + B = Business name
        //  Alt + M = "Microsoft Engineer"
        //  Alt + P = "the Microsoft Product Group"
        //  Alt + O = Business name + " is experiencing an issue with " + TechBox.Text + " where users are unable to "
        //  Alt + R = " is currently reviewing logs and will update " + Business name + " when they have finished their analysis."
        private void OVKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            StringBuilder sb = new StringBuilder();

            if (e.Key == System.Windows.Input.Key.F1)
                OVBox.Text = OVBox.Text + " " + BusNameBox.Text;
            else if (e.Key == System.Windows.Input.Key.F2)
                OVBox.Text = OVBox.Text + "Microsoft Engineer";
            else if (e.Key == System.Windows.Input.Key.F3)
                OVBox.Text = OVBox.Text + "the Microsoft Product Group";
            else if (e.Key == System.Windows.Input.Key.F5)
            {
                OVBox.Text = BusNameBox.Text + " is experiencing an issue with " + TechBox.Text + " where users are unable to ";
                OVBox.CaretIndex = OVBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F4)
                OVBox.Text = OVBox.Text + " The Microsoft Engineer is currently reviewing logs and will update " + BusNameBox.Text + " when they have finished their analysis.";
        }

        private void BIKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.F1)
                BIBox.Text = BIBox.Text + BusNameBox.Text;
            else if (e.Key == System.Windows.Input.Key.F2)
                BIBox.Text = BIBox.Text + "Microsoft Engineer";
            else if (e.Key == System.Windows.Input.Key.F3)
                BIBox.Text = BIBox.Text + "the Microsoft Product Group";
            else if (e.Key == System.Windows.Input.Key.F4)
                BIBox.Text = BIBox.Text + "The Microsoft Engineer is currently reviewing logs and will update " + BusNameBox.Text + " when they have finished their analysis.";
        }

        private void CSKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.F1)
            {
                CSBox.Text = CSBox.Text + BusNameBox.Text;
                CSBox.CaretIndex = CSBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F2)
            {
                CSBox.Text = CSBox.Text + "Microsoft Engineer";
                CSBox.CaretIndex = CSBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F3)
            {
                CSBox.Text = CSBox.Text + "the Microsoft Product Group";
                CSBox.CaretIndex = CSBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F4)
            {
                CSBox.Text = CSBox.Text + "The Microsoft Engineer is currently reviewing logs and will update " + BusNameBox.Text + " when they have finished their analysis.";
                CSBox.CaretIndex = CSBox.Text.Length;
            }
        }

        private void APMSKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.F1)
                MSAPBox.Text = MSAPBox.Text + BusNameBox.Text;
            else if (e.Key == System.Windows.Input.Key.F2)
                MSAPBox.Text = MSAPBox.Text + "Microsoft Engineer";
            else if (e.Key == System.Windows.Input.Key.F3)
                MSAPBox.Text = MSAPBox.Text + "the Microsoft Product Group";
            else if (e.Key == System.Windows.Input.Key.F4)
                MSAPBox.Text = MSAPBox.Text + "The Microsoft Engineer is currently reviewing logs and will update " + BusNameBox.Text + " when they have finished their analysis.";
        }

        private void APBSKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.F1)
                BusAPBox.Text = BusAPBox.Text + BusNameBox.Text;
            else if (e.Key == System.Windows.Input.Key.F2)
                BusAPBox.Text = BusAPBox.Text + "Microsoft Engineer";
            else if (e.Key == System.Windows.Input.Key.F3)
                BusAPBox.Text = BusAPBox.Text + "the Microsoft Product Group";
            else if (e.Key == System.Windows.Input.Key.F4)
                BusAPBox.Text = BusAPBox.Text + "The Microsoft Engineer is currently reviewing logs and will update " + BusNameBox.Text + " when they have finished their analysis.";
        }

        private void InternalKeyDownFunctions( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if (e.Key == System.Windows.Input.Key.F1)
            {
                InternalBox.Text = "Case Owners Engaged:\n" + EngiBox.Text + "(" + EngiAliasBox.Text + ") | Case Owner" + InternalBox.Text + InternalBox.Text;
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F2)
            {
                InternalBox.Text = InternalBox.Text + "\n\nCurrent Status: locating an engineer";
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F3)
            {
                InternalBox.Text = InternalBox.Text + "\n\nCurrent Status: waiting on PG";
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F4)
            {
                InternalBox.Text = InternalBox.Text + "\n\nCurrent Status: reviewing logs";
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F5)
            {
                InternalBox.Text = InternalBox.Text + "\n\nCurrent Status: waiting for cx reengagement";
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
            else if (e.Key == System.Windows.Input.Key.F6)
            {
                Case c = SRList.SelectedItem as Case;
                Communication latestComm = c._Comms.ElementAt(c._Comms.Count - 1);

                InternalBox.Text = InternalBox.Text + "\n\nLatest Comm - " + latestComm.Type + " - " + latestComm.TimeToShow + " - " + latestComm.Content;
                InternalBox.CaretIndex = InternalBox.Text.Length;
            }
        }

        private void HOTemplateClick( object sender, RoutedEventArgs e )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Handover Note:");
            sb.AppendLine("- Case Active: " + CaseActiveCheck.IsChecked);
            sb.AppendLine("- Next Immediate Step: " + NextStepBox.Text);
            sb.AppendLine("- Engineer Engaged: " + EngiEngagedCheck.IsChecked);
            sb.AppendLine("- Last update by engineer: ");
            sb.AppendLine("- Additional Note: " + NoteBox.Text);
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Copied the following to clipboard:\n\n" + Clipboard.GetText(), "Copied to clipboard", MessageBoxButton.OK, MessageBoxImage.None);
        }

        //private void SearchAndModify( string toSearch, string toReplace )
        //{
        //    List<string> Lines = InternalBox.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList<string>();
        //    string newText = String.Empty;

        //    if (InternalBox.Text.Contains(toSearch))
        //    {
        //        for (int i = 0; i < Lines.Count; i++)
        //        {
        //            if (Lines[i].Contains(toSearch))
        //            {
        //                Lines[i] = toReplace;
        //            }
        //        }

        //        Lines.ForEach(str => newText += str + "\n");
        //        InternalBox.Text = newText;
        //    }

        //    else
        //    {
        //        InternalBox.Text = InternalBox.Text + toReplace;
        //    }                
        //}
    }
}