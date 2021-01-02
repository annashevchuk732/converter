using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Converter
{
    public class ValuteInfo
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string ChacCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }
}
