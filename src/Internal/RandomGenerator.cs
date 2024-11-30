using System;
using static RandEx.RandomEx;

namespace RandEx.Internal;

internal class RandomGenerator
{
    internal const ulong Multiplier = 6364136223846793005UL;

    internal static ulong GenerateSeed() =>
        ((ulong)DateTime.UtcNow.Ticks << 32) | (ulong)Environment.TickCount64;

    internal static ulong RotateRight(ulong value, int count) =>
        unchecked((value >> (count % 32)) | (value << (32 - (count % 32))));

    internal static ulong GenerateRandom()
    {
        ulong state = _threadState.Value;
        ulong oldState = state;
        _threadState.Value = oldState * Multiplier + _sequence.Value;
        ulong xorshifted = ((oldState ^ (oldState >> 18)) >> 27);
        ulong rot = oldState >> 59;
        return RotateRight(xorshifted, (int)rot);
    }
}