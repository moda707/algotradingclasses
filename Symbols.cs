using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class Symbols
    {
        public Symbols(string _Name, string _InsCode)
        {
            Symbol = _Name;
            InsCode = _InsCode;
        }

        public bool Selected { get; set; }
        public string InsCode { get; set; }
        public string Symbol { get; set; }

    }
}
