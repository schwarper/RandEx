using System.Collections.Frozen;
using System.Collections.Generic;

namespace RandEx.Utils;

public class RandomStringCharacter
{
    public static readonly IReadOnlyDictionary<RandomStringOptions, char[]> RandomCharacterSets = new Dictionary<RandomStringOptions, char[]>()
    {
        { RandomStringOptions.IncludeLowercaseLetters, "abcdefghijklmnopqrstuvwxyz".ToCharArray() },
        { RandomStringOptions.IncludeUppercaseLetters, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() },
        { RandomStringOptions.IncludeNumbers, "0123456789".ToCharArray() },
        { RandomStringOptions.IncludeSpecialCharacters, "!@#$%^&*()_+-=[]{}|;:,.<>?/".ToCharArray() }
    }.ToFrozenDictionary();
}