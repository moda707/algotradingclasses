using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class Indicator
    {
        public string InsCode { get; set; }
        public string Name { get; set; }
        public int SDEven { get; set; }
        public int SHEven { get; set; }
        public int EDEven { get; set; }
        public int EHEven { get; set; }
        public int Scope { get; set; }
        public int MPeriod { get; set; }        
        
        public List<Parameter> IndParameters { get; set; }

        string sqlCmd;
        public Indicator(string N, List<Parameter> P)
        {
            Name = N;
            IndParameters = new List<Parameter>();
            IndParameters = P;
        }

        public string getQuery()
        {
            sqlCmd = @"SELECT T.DEven, T.HEven, T.price, ((CASE WHEN T.Buy>0 THEN 1 ELSE 0 END) + (CASE WHEN T.Sell>0 THEN -1 ELSE 0 END)) Signal, T.PMeDem, T.PMeOf, T.QTitMeDem, T.QTitMeOf FROM TseTrade.dbo.fn_AT_IND_" + Name + "_SIGNALS(" + InsCode + "," + Scope + "," + SDEven + "," + SHEven + "," + EDEven + "," + EHEven;
            
            foreach (Parameter a in IndParameters.OrderBy(t=>t.Order))
            {
                sqlCmd += "," + a.Value;
            }
            sqlCmd += ") T ORDER BY T.DEven,T.HEven";
            return sqlCmd;
        }

    }

    public enum IndicatorNames
    {
        Alligator,
        MACD,
        RSI,
        CCI,
        BolingerBand,
        Fractal,
        Stochastic,
        MACross,
        Ichimoku
    }
}

