using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;


using Accord.Neuro;
using Accord.Math;
using Accord.Math.Optimization;
using Accord.Statistics;
using Accord.Neuro.Learning;


namespace BaseClasses
{
    [Serializable]
    public class Neural_Network
    {
        public ActivationNetwork Network;
        public string SDEven;
        public string SHEven;
        public string EDEven;
        public string EHEven;
        public double TrainError;
        public double TestError;
        public Symbols Symbol;
        public int Periodicity;
        public int MPeriod;//Lookback Period

       /*         
        public void SerializeMe(string fileName)
        {
            Indicator_Combination.Serializer.SerializeNeuralNetwork(this, fileName);
        }

        public static Neural_Network DeserializeFromFile(string fileName)
        {
            var r = new Neural_Network();
            r = Serializer.DeserializeNeuralNetwork(fileName);
            return r;
        }
        */
    }

    public static class Serializer
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>

        public static void SerializeNeuralNetwork(Neural_Network l, string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, l);
                    stream.Close();
                }
            }
            catch (IOException)
            {
            }
        }


        public static Neural_Network DeserializeNeuralNetwork(string fileName)
        {

            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {

                    BinaryFormatter bin = new BinaryFormatter();
                    var h = new Neural_Network();
                    h = (Neural_Network)bin.Deserialize(stream);
                    stream.Close();
                    return h; ;
                }
            }
            catch (IOException)
            {
                return null;
            }
        }
                
    }
}
