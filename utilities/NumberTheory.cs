using System.Numerics;

namespace AoC.Utilities;

public static class NumberTheory<T> where T : INumber<T>
{
    public static T LeastCommonMultiple(params T[] numbers) => numbers.Aggregate(LCM);
    public static T LeastCommonMultiple(IEnumerable<T> numbers) => numbers.Aggregate(LCM);

    private static T LCM(T a, T b)
    {
        return (a * b) / GCD(a, b);
    }

    public static T GreatestCommonDivisor(params T[] numbers) => numbers.Aggregate(GCD);
    public static T GreatestCommonDivisor(IEnumerable<T> numbers) => numbers.Aggregate(GCD);

    private static T GCD(T a, T b)
    {
        return b == default ? a : GCD(b, a % b);
    }
}