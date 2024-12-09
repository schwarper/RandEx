using System;
using System.Threading;

namespace RandEx.Internal;

internal class RandomGenerator
{
    /// <summary>
    /// Multiplier used in the PCG algorithm for randomness generation.
    /// </summary>
    internal const ulong Multiplier = 6364136223846793005UL;

    /// <summary>
    /// Thread-local storage for random number generation state, ensuring thread-safety.
    /// Each thread has its own unique random seed, enhancing randomness.
    /// </summary>
    internal static readonly ThreadLocal<ulong> _threadState = new(GenerateSeed);

    /// <summary>
    /// Thread-local storage for sequence generation in the random number generator.
    /// Each thread has its own unique sequence, used in PCG algorithm for randomness.
    /// </summary>
    internal static readonly ThreadLocal<ulong> _sequence = new(GenerateSeed);

    /// <summary>
    /// Thread-local storage for Gaussian random value caching.
    /// Stores whether a spare cached Gaussian value is available and the cached value itself.
    /// </summary>
    /// <remarks>
    /// This helps to avoid repeated calculations in a Gaussian distribution generation.
    /// </remarks>
    internal static readonly ThreadLocal<(bool HasSpare, double Spare)> _gaussianState = new(() => (false, 0.0), true);

    /// <summary>
    /// Generates a seed value for random number generation based on the current time and thread ID.
    /// </summary>
    /// <returns>A 64-bit unsigned integer seed value.</returns>
    internal static ulong GenerateSeed() => (ulong)(Environment.TickCount64 ^ Environment.CurrentManagedThreadId);

    /// <summary>
    /// Generates a random 64-bit unsigned integer using the PCG (Permutation Congruential Generator) algorithm.
    /// </summary>
    /// <returns>A random 64-bit unsigned integer.</returns>
    internal static ulong GenerateRandom()
    {
        _sequence.Value ^= _sequence.Value >> 12;
        _sequence.Value ^= _sequence.Value << 25;
        _sequence.Value ^= _sequence.Value >> 27;

        _threadState.Value = _threadState.Value * Multiplier + _sequence.Value;

        return _threadState.Value;
    }
}