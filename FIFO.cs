using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class FIFO
    {
        public List<F_Item> FList;
        private int myRow;
        public FIFO(int RowCount)
        {
            FList = new List<F_Item>(RowCount);
            myRow = RowCount;
        }

        public void Push(F_Item NewItem)
        {
            if (FList.Count == myRow)
                FList.RemoveAt(0);
            FList.Add(NewItem);

        }

        public double GetMeanValue()
        {
            return FList.Average(x => x.NNO);

        }

        public double GetMeanDiff()
        {
            double MeanDiff = 0;
            if (FList.Count() <= 1) return 0;

            for (int i = 1; i < FList.Count(); i++)
            {
                MeanDiff += Math.Abs(FList[i].NNO - FList[i - 1].NNO);
            }
            return MeanDiff / (myRow - 1);
        }
    }

    public class F_Item
    {
        public string DEven;
        public string HEven;
        public double NNO;
    }
}
