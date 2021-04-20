using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    public class Reminder
    {
        public string ReminderText { get; set; }
        public DateTime ReminderTime { get; set; }
        public string TimeToShow
        {
            get
            {
                return ReminderTime.ToString("hh:mm tt");
            }
        }
    }
}
