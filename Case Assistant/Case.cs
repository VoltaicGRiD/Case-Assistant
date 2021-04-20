using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Case_Assistant
{
    public class Case
    {
        public ObservableCollection<Communication> _Comms = new ObservableCollection<Communication>();
        public ObservableCollection<Communication> Comms { get { return _Comms; } }
        public ObservableCollection<Labors> _Labor = new ObservableCollection<Labors>();
        public ObservableCollection<Labors> Labor { get { return _Labor; } }

        public DateTime SixHour { get; set; }

        public DateTime DMEmailSent { get; set; }

        public int CalcLabor = 0;
        public int TimeLogged
        {
            get
            {
                foreach (Labors l in this.Labor)
                {
                    CalcLabor += l.LaborTime;
                }

                return CalcLabor;
            }
        }

        public string SRNum { get; set; }
        public string BusName { get; set; }
        public string TAM { get; set; }
        public string TAMAlias { get; set; }
        public string BTAM { get; set; }
        public string BTAMAlias { get; set; }
        public string Engi { get; set; }
        public string EngiAlias { get; set; }
        public string DM { get; set; }
        public string Tech { get; set; }
        public string RaveNum { get; set; }
        public string IcMNum { get; set; }
        public string Link { get; set; }
        public string BridgeLink { get; set; }
        public string NextStep { get; set; }
        public string AddNote { get; set; }
        public bool Political { get; set; }
        public bool Security { get; set; }
        public bool DataComp { get; set; }
        public bool CustomerEngaged { get; set; } = true;
        public bool PG { get; set; }
        public bool TAMEngaged { get; set; }
        public bool EngiEngaged { get; set; }
        public bool ResourceChase { get; set; }
        public bool CaseActive { get; set; } = true;
        public string CxName { get; set; }
        public string CxPhone { get; set; }
        public string CxEmail { get; set; }
        public string Contacts { get; set; }
        public enum ContactPreference
        {
            Phone = 1,
            Email = 2,
            Skype = 3,
            WebEx = 4
        }
        public ContactPreference Preference { get; set; }
        public enum _Temperature
        {
            Cold,
            Cool,
            Mild,
            Warm,
            Hot,
            Fire
        }
        public IList<_Temperature> Temperatures
        {
            get
            {
                return Enum.GetValues(typeof(_Temperature)).Cast<_Temperature>().ToList<_Temperature>();
            }
        }
        public _Temperature Temperature { get; set; }
        public enum _ActionUpon
        {
            Customer,
            Engineer,
            PG,
            Microsoft,
            Other
        }
        public IList<_ActionUpon> ActionUpons
        {
            get
            {
                return Enum.GetValues(typeof(_ActionUpon)).Cast<_ActionUpon>().ToList<_ActionUpon>();
            }
        }
        public _ActionUpon ActionUpon { get; set; }
        public DateTime TasksDue { get; set; }
        //public DateTime EngageTime { get; set; }
        public string OV { get; set; }
        public string BI { get; set; }
        public string CS { get; set; }
        public string MSAP { get; set; }
        public string BSAP { get; set; }
        public string Internal { get; set; }
        public string Personal { get; set; }

        public string BusNameAP
        {
            get
            {
                return this.BusName + ":";
            }
        }

        public string GetSaveName
        {
            get
            {
                string final = "";
                if (this.SRNum.Length == 15 && BusName.Length > 0)
                {
                    if (BusName.Length < 8)
                    {
                        final = SRNum.Substring(11, 4) + " - " + BusName.Substring(0, BusName.Length);
                        return final;
                    }
                    else
                    {
                        final = SRNum.Substring(11, 4) + " - " + BusName.Substring(0, 8);
                        return final;
                    }
                }
                else
                    return "ERROR";
            }
        }
    }
}
