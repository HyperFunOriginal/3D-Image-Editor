using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RNGenerate
{
    internal static long seed = DateTime.UtcNow.Ticks;
    internal static long seed2 = (DateTime.Now.Second + DateTime.Now.DayOfYear * 1000) * (long)DateTime.Now.TimeOfDay.TotalSeconds + DateTime.Now.Day;

    public static void SetSeeds(long a, long b)
    {
        seed = a;
        seed2 = b;
    }
    public static double NextNormalDist(double standardDeviation = 1d, double minimum = double.MinValue, double maximum = double.MaxValue, double mean = 0d)
    {
        double value = NextDouble();
        value = Math.Log((1d + value) / (1d - value)) * 0.5512594072417845d * standardDeviation + mean;
        return Math.Max(Math.Min(value, maximum), minimum);
    }
    public static double NextNormalDist()
    {
        double value = NextDouble();
        return Math.Log((1d + value) / (1d - value)) * 0.5512594072417845d;
    }
    public static int NextInt(int minimum, int maximum) => (int)(NextUInt() % (maximum - minimum + 1)) + minimum;
    public static int NextInt()
    {
        Next();
        return (int)(((seed & 281470681743360) + (seed2 & 4294901760)) >> 16);
    }
    public static uint NextUInt()
    {
        Next();
        return (uint)(((seed & 281470681743360) + (seed2 & 4294901760)) >> 16);
    }
    public static long NextLong()
    {
        Next();
        return seed ^ seed2;
    }
    public static double NextDouble()
    {
        Next();
        double a = seed;
        double b = seed2;
        a *= 1.08420217248550443400745280086994171142578125e-19;
        b *= 1.1758790184392223605718895838186412751564794921875e-38;
        return a + b;
    }
    public static double NextDouble(double minimum, double maximum) => NextDouble01() * (maximum - minimum) + minimum;
    public static double NextDouble01() => Math.Abs(NextDouble());
    public static decimal NextDecimal01() => Math.Abs(NextDecimal());
    public static decimal NextDecimal()
    {
        Next();
        decimal a = seed;
        decimal b = seed2;
        a *= 1.08420217248550443400745280086994171142578125e-19m;
        b *= 1.1758790184392223605718895838186412751564794921875e-38m;
        return a + b;
    }
    public static void Next()
    {
        long a = seed ^ seed2 + (seed * seed2) - 3718947842112;
        long b = (seed & seed2) + (seed | seed2) + 41278471811227;

        a *= (b << 32) | (b >> 32);
        a += seed >> 23;

        b *= seed + 1512412421;
        b -= seed2 >> 16;

        seed = b;
        seed2 = a;
    }
}
