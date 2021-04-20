using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Case_Assistant
{
    public class Communication
    {
        public enum _Type
        {
            Call = 1,
            Email = 2,
            Bridge = 3,
            Skype = 4,
            WebEx = 5
        }
        public _Type Type { get; set; }
        public DateTime Time { get; set; } 
        public string TimeToShow
        {
            get
            {
                return Time.Hour + ":" + Time.Minute + " UTC";
            }
        }
        public string Name { get; set; }
        public string FullContent { get; set; }
        public string Content { get; set; }

        public string Description { get; set; }
    }
}
