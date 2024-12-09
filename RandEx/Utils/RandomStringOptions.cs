using System;

namespace RandEx.Utils;

/// <summary>
/// Specifies the options for generating random strings by including different character sets.
/// This enumeration is used to define which types of characters should be included in the string.
/// </summary>
[Flags]
public enum RandomStringOptions
{
    /// <summary>
    /// Includes lowercase letters (a-z) in the random string.
    /// </summary>
    IncludeLowercaseLetters = 1,

    /// <summary>
    /// Includes uppercase letters (A-Z) in the random string.
    /// </summary>
    IncludeUppercaseLetters = 2,

    /// <summary>
    /// Includes numeric characters (0-9) in the random string.
    /// </summary>
    IncludeNumbers = 4,

    /// <summary>
    /// Includes special characters (e.g., punctuation and symbols) in the random string.
    /// </summary>
    IncludeSpecialCharacters = 8
}
