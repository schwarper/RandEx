using System.Collections.Frozen;
using System.Collections.Generic;

namespace RandEx.Utils;

/// <summary>
/// Provides predefined character sets for generating random strings based on the selected <see cref="RandomStringOptions"/>.
/// </summary>
public class RandomStringCharacter
{
    /// <summary>
    /// A frozen dictionary mapping <see cref="RandomStringOptions"/> to corresponding character arrays.
    /// This dictionary defines the character sets available for generating random strings.
    /// </summary>
    public static readonly FrozenDictionary<RandomStringOptions, char[]> RandomCharacterSets =
        new Dictionary<RandomStringOptions, char[]>()
        {
            { RandomStringOptions.IncludeLowercaseLetters, "abcdefghijklmnopqrstuvwxyz".ToCharArray() },
            { RandomStringOptions.IncludeUppercaseLetters, "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() },
            { RandomStringOptions.IncludeNumbers, "0123456789".ToCharArray() },
            { RandomStringOptions.IncludeSpecialCharacters, "!@#$%^&*()_+-=[]{}|;:,.<>?/".ToCharArray() }
        }.ToFrozenDictionary();
}
