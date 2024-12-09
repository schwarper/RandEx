using System;
using System.Collections.Generic;
using RandEx.Utils;
using static RandEx.Utils.RandomStringCharacter;
using static RandEx.Internal.RandomGenerator;
using System.Buffers.Binary;

namespace RandEx;

/// <summary>
/// Provides a collection of utility methods for generating random values, such as numbers, strings, colors, 
/// and other data types, with additional functionality like shuffling collections or generating random geographic coordinates.
/// </summary>
/// <remarks>
/// This class includes a variety of methods for generating random data, including integers, floats, doubles, booleans,
/// strings, and colors. It supports seeding for reproducibility and offers customizations such as selecting
/// specific character sets or range limits. Suitable for general-purpose random value generation.
/// </remarks>
public static class RandomEx
{
    /// <summary>
    /// Sets a custom seed for the random number generator.
    /// </summary>
    /// <param name="seed">The seed to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="seed"/> is negative or zero.
    /// </exception>
    public static void SetSeed(int seed)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seed);

        _threadState.Value = (ulong)seed * Multiplier;
        _sequence.Value = GenerateSeed();
    }

    /// <summary>
    /// Generates a random byte between 0 and 255.
    /// </summary>
    /// <returns>A random byte between 0 and 255.</returns>
    public static byte GetRandomByte() => (byte)GetRandomInt(maxValue: 256);

    /// <summary>
    /// Generates a random boolean value: true or false.
    /// </summary>
    /// <returns>A random boolean value: true or false.</returns>
    public static bool GetRandomBool() => (GenerateRandom() & 1) != 0;

    /// <summary>
    /// Generates a random color represented as a byte tuple (R, G, B).
    /// </summary>
    /// <returns>A random color represented as a byte tuple (R, G, B).</returns>
    public static (byte R, byte G, byte B) GetRandomByteColor() => (GetRandomByte(), GetRandomByte(), GetRandomByte());

    /// <summary>
    /// Generates a random integer within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number.</param>
    /// <returns>A random integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="minValue"/> is greater than or equal to <paramref name="maxValue"/>.
    /// </exception>
    public static int GetRandomInt(int minValue = 0, int maxValue = int.MaxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minValue, maxValue);

        int range = maxValue - minValue;

        if ((range & (range - 1)) == 0)
        {
            return minValue + (int)(GenerateRandom() & ((ulong)range - 1));
        }

        ulong bits, result;

        do
        {
            bits = GenerateRandom();
            result = bits % (ulong)range;
        } while (bits - result + (ulong)(range - 1) < bits);

        return minValue + (int)result;
    }

    /// <summary>
	/// Generates a random float number in the specified range.
	/// </summary>
	/// <param name="minValue">The inclusive lower bound of the range. Default is 0.</param>
	/// <param name="maxValue">The exclusive upper bound of the range. Default is float.MaxValue.</param>
	/// <returns>A random float number between minValue and maxValue.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="minValue"/> is greater than or equal to <paramref name="maxValue"/>.
    /// </exception>
    public static float GetRandomFloat(float minValue = 0.0f, float maxValue = float.MaxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minValue, maxValue);

        return minValue + (float)(GetRandomDouble() * (maxValue - minValue));
    }

    /// <summary>
    /// Generates a random double between 0.0 and 1.0.
    /// </summary>
    /// <returns>A random double between 0.0 and 1.0.</returns>
    public static double GetRandomDouble()
    {
        const ulong IEEEPrecisionLimit = (1UL << 53) - 1;
        const double ScaleFactor = 1.0 / (1UL << 53);

        ulong randomValue = GenerateRandom();
        randomValue &= IEEEPrecisionLimit;

        return randomValue * ScaleFactor;
    }

    /// <summary>
    /// Generates a random value from a Gaussian (normal) distribution with a mean of 0 and a standard deviation of 1.
    /// </summary>
    /// <returns>A random double from a standard normal distribution.</returns>
    public static double GetRandomGaussian()
    {
        var (HasSpare, Spare) = _gaussianState.Value;

        if (HasSpare)
        {
            _gaussianState.Value = (false, 0.0);
            return Spare;
        }

        double u1, u2, s;

        do
        {
            u1 = 2.0 * GetRandomDouble() - 1.0;
            u2 = 2.0 * GetRandomDouble() - 1.0;
            s = u1 * u1 + u2 * u2;
        }
        while (s >= 1.0 || s == 0.0);

        double multiplier = Math.Sqrt(-2.0 * Math.Log(s) / s);
        _gaussianState.Value = (true, u2 * multiplier);
        return u1 * multiplier;
    }

    /// <summary>
    /// Generates a random byte array of the specified length.
    /// </summary>
    /// <param name="length">The length of the byte array to generate.</param>
    /// <returns>A byte array filled with random values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="length"/> is negative or zero.
    /// </exception>
    public static byte[] GetRandomBytes(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");
        }

        var bytes = new byte[length];
        Span<byte> buffer = bytes.AsSpan();

        for (int i = 0; i < buffer.Length; i += sizeof(ulong))
        {
            ulong value = GenerateRandom();
            BinaryPrimitives.WriteUInt64LittleEndian(buffer[i..], value);
        }

        return bytes;
    }

    /// <summary>
    /// Returns a random value from the specified enumeration type.
    /// </summary>
    /// <typeparam name="T">The enumeration type from which to select a random value.</typeparam>
    /// <returns>A randomly selected value of the specified enumeration type.</returns>
    public static T GetRandomEnumValue<T>() where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        return values[GetRandomInt(maxValue: values.Length)];
    }

    /// <summary>
    /// Returns a random element from the given collection.
    /// </summary>
    /// <param name="collection">The collection to select a random element from.</param>
    /// <returns>A random element from the collection.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="collection"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="collection"/> is empty.
    /// </exception>
    public static T GetRandomElement<T>(IList<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (collection.Count == 0)
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(collection));
        }

        return collection[GetRandomInt(0, collection.Count)];
    }

    /// <summary>
    /// Generates a random DateTime between the specified minDate and maxDate.
    /// </summary>
    /// <param name="minDate">The lower bound for the random DateTime. default is DateTime.MinValue.</param>
    /// <param name="maxDate">The upper bound for the random DateTime. default is DateTime.MaxValue.</param>
    /// <returns>A random DateTime value between minDate and maxDate.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minDate"/> is greater than or equal to <paramref name="maxDate"/>.
    /// </exception>    
    public static DateTime GetRandomDateTime(DateTime minDate = default, DateTime maxDate = default)
    {
        if (minDate == default) minDate = DateTime.MinValue;
        if (maxDate == default) maxDate = DateTime.MaxValue;

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minDate, maxDate);

        TimeSpan range = maxDate - minDate;
        return minDate.AddTicks((long)(GetRandomDouble() * range.Ticks));
    }


    /// <summary>
    /// Generates a random character from a specified inclusive character range, based on Unicode values.
    /// The default range is from space (' ') to tilde ('~'), covering printable ASCII characters.
    /// </summary>
    /// <param name="minChar">The inclusive lower bound of the character range. Default is the space character (' ').</param>
    /// <param name="maxChar">The inclusive upper bound of the character range. Default is the tilde character ('~').</param>
    /// <returns>A random character within the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="minChar"/> is greater than or equal to <paramref name="maxChar"/>.
    /// </exception>

    public static char GetRandomChar(char minChar = ' ', char maxChar = '~')
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minChar, maxChar);

        return (char)GetRandomInt(minChar, maxChar + 1);
    }

    /// <summary>
    /// Generates a random string of the specified length using the character sets defined by <see cref="RandomStringOptions"/>.
    /// By default, includes lowercase letters, uppercase letters, and numbers.
    /// </summary>
    /// <param name="length">The desired length of the generated string. Must be greater than zero.</param>
    /// <param name="options">
    /// Specifies the character sets to include in the string using a combination of <see cref="RandomStringOptions"/>.
    /// If no options are provided, the default includes lowercase letters, uppercase letters, and numbers.
    /// </param>
    /// <returns>A random string of the specified length generated from the selected character sets.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="length"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if no valid character sets are selected based on the provided <paramref name="options"/>.
    /// </exception>
    public static string GetRandomString(int length, RandomStringOptions options = RandomStringOptions.IncludeLowercaseLetters | RandomStringOptions.IncludeUppercaseLetters | RandomStringOptions.IncludeNumbers | RandomStringOptions.IncludeSpecialCharacters)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        List<char> chars = [];

        foreach (var option in RandomCharacterSets)
        {
            if (options.HasFlag(option.Key))
            {
                chars.AddRange(option.Value);
            }
        }

        if (chars.Count == 0)
        {
            throw new ArgumentException("No valid character sets were selected based on the provided RandomStringOptions.", nameof(options));
        }

        Span<char> result = stackalloc char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[GetRandomInt(0, chars.Count)];
        }

        return new string(result);
    }

    /// <summary>
	/// Generates a random geographic coordinate (latitude, longitude).
	/// </summary>
	/// <returns>A tuple representing latitude and longitude.</returns>
	public static (double Latitude, double Longitude) GetRandomGeoCoordinate()
    {
        double latitude = GetRandomDouble() * 180.0 - 90.0;
        double longitude = GetRandomDouble() * 360.0 - 180.0;

        return (Math.Round(latitude, 6), Math.Round(longitude, 6));
    }

    /// <summary>
    /// Generates a random hexadecimal color code.
    /// </summary>
    /// <returns>A hex color code (e.g., "#FF5733").</returns>
    public static string GetRandomHexColor()
    {
        return $"#{GetRandomInt(0, 256):X2}{GetRandomInt(0, 256):X2}{GetRandomInt(0, 256):X2}";
    }

    /// <summary>
    /// Shuffles the elements in the provided list.
    /// </summary>
    /// <param name="values">The list to shuffle.</param>
    public static void Shuffle<T>(List<T> values)
    {
        if (values.Count <= 1)
        {
            return;
        }

        int listCount = values.Count;

        for (int i = listCount - 1; i > 0; i--)
        {
            int j = GetRandomInt(0, i + 1);

            if (i != j)
            {
                (values[i], values[j]) = (values[j], values[i]);
            }
        }
    }
}
