using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Converter
{
    public class ValuteInfoView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ValuteInfo valuteInfo;
        public string ID 
        {
            get
            {
                return valuteInfo.ID;
            }
            set
            {
                if (valuteInfo.ID != value)
                {
                    valuteInfo.ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public string NumCode
        {
            get
            {
                return valuteInfo.NumCode;
            }
            set
            {
                if (valuteInfo.NumCode != value)
                {
                    valuteInfo.NumCode = value;
                    OnPropertyChanged("NumCode");
                }
            }
        }
        public string ChacCode
        {
            get
            {
                return valuteInfo.ChacCode;
            }
            set
            {
                if (valuteInfo.ChacCode != value)
                {
                    valuteInfo.ChacCode = value;
                    OnPropertyChanged("ChacCode");
                }
            }
        }
        public int Nominal
        {
            get
            {
                return valuteInfo.Nominal;
            }
            set
            {
                if (valuteInfo.Nominal != value)
                {
                    valuteInfo.Nominal = value;
                    OnPropertyChanged("Nominal");
                }
            }
        }
        public string Name
        {
            get
            {
                return valuteInfo.Name;
            }
            set
            {
                if (valuteInfo.Name != value)
                {
                    valuteInfo.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public double Value
        {
            get
            {
                return valuteInfo.Value;
            }
            set
            {
                if (valuteInfo.Value != value)
                {
                    valuteInfo.Value= value;
                    OnPropertyChanged("Value");
                }
            }
        }
        public double Previous
        {
            get
            {
                return valuteInfo.Previous;
            }
            set
            {
                if (valuteInfo.Previous != value)
                {
                    valuteInfo.Previous = value;
                    OnPropertyChanged("Previous");
                }
            }
        }
        public ValuteInfoView()
        {
            valuteInfo = new ValuteInfo();
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
