using System;
using System.Threading;

namespace RandEx.Internal;

internal class RandomGenerator
{
    internal const ulong Multiplier = 6364136223846793005UL;

    internal static readonly ThreadLocal<ulong> _threadState = new(GenerateSeed);
    internal static readonly ThreadLocal<(bool HasSpare, double Spare)> _gaussianState = new(() => (false, 0.0), true);
    internal static readonly ThreadLocal<ulong> _sequence = new(GenerateSeed);

    internal static ulong GenerateSeed() =>
        ((ulong)DateTime.UtcNow.Ticks << 32) | (ulong)Environment.TickCount64;

    internal static ulong RotateRight(ulong value, int count) =>
        unchecked((value >> (count & 31)) | (value << (32 - (count & 31))));

    internal static ulong GenerateRandom()
    {
        ulong state = _threadState.Value;
        ulong oldState = state;
        _threadState.Value = oldState * Multiplier + _sequence.Value;
        ulong xorshifted = (oldState ^ (oldState >> 18)) >> 27;
        ulong rot = oldState >> 59;

        return RotateRight(xorshifted, (int)rot);
    }
}