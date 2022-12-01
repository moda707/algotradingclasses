using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{

    public class Parameter
    {
        public int Order;
        public string Name;
        public double Value;

        public Parameter(int o, string n, double v)
        {
            Order = o; Name = n; Value = v;
        }

    }
}
