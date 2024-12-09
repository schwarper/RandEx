using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using RandEx;
using RandEx.Utils;
using System.Drawing;

namespace RandExTest;

public class RandomExTest
{
    private const int _iterations = 10000;
    private const double _chiSquareThreshold = 293.25;
    private const int _minUniqueCount = 100;

    private static List<T> GenerateAndValidateValues<T>(int count, Func<T> generator, Action<T>? action = null)
    {
        var list = new List<T>(count);

        for (int i = 0; i < count; i++)
        {
            var value = generator();
            list.Add(value);
            action?.Invoke(value);
        }

        return list;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void Test_SetSeed(int seed)
    {
        RandomEx.SetSeed(seed);
        var firstSequence = RandomEx.GetRandomInt();

        RandomEx.SetSeed(seed);
        var secondSequence = RandomEx.GetRandomInt();

        Assert.Equal(firstSequence, secondSequence);

        RandomEx.SetSeed(seed + 1);
        var thirdSequence = RandomEx.GetRandomInt();

        Assert.NotEqual(secondSequence, thirdSequence);
    }

    [Fact]
    public void Test_GetRandomByte()
    {
        var bytes = GenerateAndValidateValues(_iterations, RandomEx.GetRandomByte, byteValue =>
        {
            Assert.InRange(byteValue, 0, 255);
        });

        int[] byteFrequencies = new int[256];
        foreach (var b in bytes)
            byteFrequencies[b]++;

        double expectedFrequency = _iterations / 256.0;
        double chiSquare = byteFrequencies.Sum(f => (f - expectedFrequency) * (f - expectedFrequency) / expectedFrequency);

        Assert.True(chiSquare < _chiSquareThreshold,
            $"Chi-square test failed (p < 0.05). Value: {chiSquare:F2}, Expected < {_chiSquareThreshold:F2}");
    }

    [Fact]
    public void Test_GetRandomBool()
    {
        int trueCount = Enumerable.Range(0, _iterations).Count(_ => RandomEx.GetRandomBool());

        double expected = _iterations * 0.5;
        double chiSquare = Math.Pow(trueCount - expected, 2) / expected;

        Assert.True(chiSquare < 3.841, // Chi-square threshold for df=1
            $"Randomness distribution is not acceptable. Chi-Square: {chiSquare:F2}");
    }

    [Fact]
    public void Test_GetRandomByteColor()
    {
        var colors = GenerateAndValidateValues(_iterations, RandomEx.GetRandomByteColor, color =>
        {
            Assert.InRange(color.R, 0, 255);
            Assert.InRange(color.G, 0, 255);
            Assert.InRange(color.B, 0, 255);
        });

        int uniqueColors = new HashSet<(byte, byte, byte)>(colors).Count;
        Assert.True(uniqueColors > _minUniqueCount,
            $"Unique values count was {uniqueColors}, expected > {_minUniqueCount}.");
    }

    [Theory]
    [InlineData(-1000, 5000)]
    [InlineData(1000, 5000)]
    [InlineData(int.MinValue, int.MaxValue)]
    public void Test_GetRandomInt(int min, int max)
    {
        var values = GenerateAndValidateValues(_iterations, () => RandomEx.GetRandomInt(min, max), val =>
        {
            Assert.InRange(val, min, max);
        });

        int uniqueCount = new HashSet<int>(values).Count;
        Assert.True(uniqueCount > _minUniqueCount, $"Unique values count was {uniqueCount}, expected > {_minUniqueCount}.");
    }

    [Fact]
    public void Test_GetRandomDouble()
    {
        var values = GenerateAndValidateValues(_iterations, RandomEx.GetRandomDouble, val =>
        {
            Assert.InRange(val, 0.0, 1.0);
        });

        int uniqueCount = new HashSet<double>(values).Count;
        Assert.True(uniqueCount > _minUniqueCount, $"Unique values count was {uniqueCount}, expected > {_minUniqueCount}.");
    }

    [Fact]
    public void Test_GetRandomHexColor()
    {
        var hexColors = GenerateAndValidateValues(_iterations, RandomEx.GetRandomHexColor, color =>
        {
            Assert.Matches("^#[0-9A-Fa-f]{6}$", color);
        });

        int uniqueCount = new HashSet<string>(hexColors).Count;
        Assert.True(uniqueCount > _minUniqueCount, $"Unique values count was {uniqueCount}, expected > {_minUniqueCount}.");
    }

    [Theory]
    [InlineData('a', 'z')]
    [InlineData('0', '9')]
    [InlineData(' ', '~')]
    public void Test_GetRandomChar(char min, char max)
    {
        var range = max - min;

        var chars = GenerateAndValidateValues(8, () => RandomEx.GetRandomChar(min, max), ch =>
        {
            Assert.InRange(ch, min, max);
        });

        int uniqueCount = new HashSet<char>(chars).Count;
        Assert.True(uniqueCount > 4, $"Unique values count was {uniqueCount}, expected > {4}.");
    }

    [Theory]
    [InlineData(7, RandomStringOptions.IncludeLowercaseLetters)]
    [InlineData(10, RandomStringOptions.IncludeUppercaseLetters)]
    [InlineData(15, RandomStringOptions.IncludeNumbers)]
    [InlineData(30, RandomStringOptions.IncludeSpecialCharacters)]
    public void Test_GetRandomString(int length, RandomStringOptions options)
    {
        var strings = GenerateAndValidateValues(_iterations, () => RandomEx.GetRandomString(length, options), str =>
        {
            Assert.Equal(length, str.Length);
        });

        int uniqueCount = new HashSet<string>(strings).Count;
        Assert.True(uniqueCount > length / 2, $"Unique values count was {uniqueCount}, expected > {length / 2}.");
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5 })]
    public void Test_GetRandomElement(int[] collection)
    {
        var elements = GenerateAndValidateValues(_iterations, () => RandomEx.GetRandomElement(collection), element =>
        {
            Assert.Contains(element, collection);
        });

        int uniqueCount = new HashSet<int>(elements).Count;
        Assert.True(uniqueCount >= collection.Length,
            $"Unique elements count was {uniqueCount}, expected >= {collection.Length}.");
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5 })]
    public void Test_Shuffle(int[] list)
    {
        var original = list.ToList();
        var shuffled = list.ToList();

        RandomEx.Shuffle(shuffled);

        Assert.NotEqual(original, shuffled);
        Assert.Equal(original.Count, shuffled.Count);
        Assert.All(original, item => Assert.Contains(item, shuffled));
    }
}