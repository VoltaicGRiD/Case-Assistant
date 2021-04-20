using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Case_Assistant
{
    class EmailFunctions
    {
        // This copies an email template with standard Business Impact questions to the clipboard
        public bool BIEmailTemplate(string CxName)
        {
            try
            {
                Clipboard.SetText(CxName.Split(' ')[0] + ",\n\nI understand you are very busy, but this case has been alive for about 6 hours and since I am unable to contact you by phone, could you please answer the following questions " +
                "when you have a few free minutes:\n\nDo you feel that the current Microsoft Engineer is the correct person to be able to resolve the issue?\nHas the impact to your business changed (i.e. affecting more users/more departments) " +
                "from what was mentioned previously?\nDo you have any other administrative-oriented questions or concerns related to this case that I can answer?\n\nThank you,\n");
                MessageBox.Show("Email template copied to clipboard.\n\nPress Control + V to paste.", "Copied", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        // This copies an email template with standard Triage questions to the clipboard
        public bool TriageEmailTemplate(string CxName)
        {
            try
            {
                Clipboard.SetText(CxName.Split(' ')[0] + ",\n\nI understand you are very busy; however, could you please answer the following questions " +
                "when you have a few free minutes:\n\nDo you feel that the current Microsoft Engineer(s) is the correct person to be able to resolve the issue?\nHas the impact to your business changed (i.e. affecting more users/more departments) " +
                "from what was mentioned previously?\nDo you have any other administrative-oriented questions or concerns related to this case that I can answer?\n\nThank you,\n");
                MessageBox.Show("Email template copied to clipboard.\n\nPress Control + V to paste.", "Copied", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        // This copies an email template with a generic -Lowering Visibility- email to the clipboard
        public bool LoweringEmailTemplate(string CxName)
        {
            try
            {
                Clipboard.SetText(CxName.Split(' ')[0] + ",\n\nThank you for your time on our call, with the permission that you have given Microsoft, we will proceed to lower the severity of the case. Should you " +
                "need to re-raise the severity to Critical (Severity A) please call your region's Premier Hotline number and reference the case number in the subject heading of all of our emails.\n\nYou should recieve an official email " +
                "regarding this change in severity shortly.\n\nThank you,\n");
                MessageBox.Show("Email template copied to clipboard.\n\nPress Control + V to paste.", "Copied", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        // This copies an email template with a generic -Archiving Visibility- email to the clipboard
        public bool ArchivingEmailTemplate(string CxName)
        {
            try
            {
                Clipboard.SetText(CxName.Split(' ')[0] + ",\n\nThank you for your time on our call, with the permission that you have given Microsoft, we will proceed to archive the case. Should you " +
                "need to re-open the case, please call your region's Premier Hotline number and reference the case number in the subject heading of all of our emails.\n\nYou should recieve an official email " +
                "regarding this change shortly.\n\nThank you,\n");
                MessageBox.Show("Email template copied to clipboard.\n\nPress Control + V to paste.", "Copied", MessageBoxButton.OK, MessageBoxImage.None);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        // This auto-generates a Resource Issue email, pre-populated with necessary information and using standard RI Email Template
        public bool RIEmailGenerate(Case c)
        {
            try
            {
                c.DMEmailSent = DateTime.UtcNow;
                Outlook.Application app = new Outlook.Application();
                Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);
                mailItem.Subject = "DM Engaged for SR# " + Regex.Replace(c.SRNum, @"\s+", "") + " - " + c.BusName;
                mailItem.To = c.DM;
                mailItem.CC = "crit365; casemail; critchase;" + c.TAMAlias + "; " + c.BTAMAlias + "; ";
                mailItem.Body = "Hello DM Team,\n\nWe are reaching out to your DM / Incident manager group as we've checked in MSSolve and can't see an indication of your engagement.\n" +
                    "If you are currently engaged in trying to locate an engineer for this case can you please update MSSolve as per your process Premier Sev A Escalation Process for Duty Managers and CritSit Managers.\n\n" +
                    "If you are not currently engaged can you please ensure your group is aware and start to locate an engineer for this case as soon as possible.\n\n";
                mailItem.Display(true);

                return true;
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        //  This sets an existing Outlook email rule to look for SR Numbers provided
        public bool SetEmailRules(Object[] textCondition)
        {
            try
            {
                Outlook.Application oApp = new Outlook.Application();
                Outlook.NameSpace oNamespace = oApp.GetNamespace("MAPI");
                Outlook.MAPIFolder vEmailFolder = null;
                Outlook.MAPIFolder CaseFolder = null;
                Outlook.Rules rules = null;
                Outlook.Rule moveRule = null;

                foreach (Outlook.MAPIFolder folder in oNamespace.Folders)
                {
                    if (folder.Name.Contains("@microsoft.com") && !folder.Name.Contains("Public"))
                    {
                        vEmailFolder = folder;
                        Debug.WriteLine("Initializing master inbox as : " + folder.Name);
                    }
                }

                foreach (Outlook.MAPIFolder folder in vEmailFolder.Folders)
                {
                    if (folder.Name.Contains("Case"))
                    {
                        CaseFolder = folder;
                        Debug.WriteLine("Initializing slave inbox as : " + folder.Name);
                    }
                }

                rules = oApp.Session.DefaultStore.GetRules();
                foreach (Outlook.Rule r in rules)
                {
                    if (r.Name.Contains("Case"))
                    {
                        moveRule = r;
                    }
                }

                if (moveRule == null)
                {
                    moveRule = rules.Create("Cases", Outlook.OlRuleType.olRuleReceive);
                }

                moveRule.Conditions.BodyOrSubject.Text = textCondition;
                moveRule.Conditions.BodyOrSubject.Enabled = true;

                try
                {
                    rules.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
    }
}
