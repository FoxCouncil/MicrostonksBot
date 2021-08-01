// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using System;

namespace MicrostonksBot
{
    public static class Dice
    {
        private static readonly Random _rng = new();

        public static double Roll()
        {
            return _rng.NextDoubleLinear(-1.5, 1.5);
        }

        public static double NextDoubleLinear(this Random random, double minValue, double maxValue)
        {            
            double sample = random.NextDouble();

            return (maxValue * sample) + (minValue * (1d - sample));
        }
    }
}
