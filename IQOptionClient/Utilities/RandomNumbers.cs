using System;
using IQOptionClient.Time;

namespace IQOptionClient.Utilities
{
    public class RandomNumbers : IRandomNumbers
    {
        private readonly Random _rnd = new Random();

        public int GenerateValue(int min, int max)
        {
            return _rnd.Next(min, max);
        }

        public int GenerateValue()
        {
            return this.GenerateValue(0, Int32.MaxValue);
        }
    }
}
