﻿namespace Birch.Extensions;

public static class RandomExtensions
{
    public static double NextDouble(this Random random, double min, double max) =>
        random.NextDouble() * (max - min) + min;

    public static float NextFloat(this Random random) => (float)random.NextDouble();

    public static float NextFloat(this Random random, float min, float max) => random.NextFloat() * (max - min) + min;
}