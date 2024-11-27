
# RandEx

RandEx is a library designed to provide advanced randomisation functions in C#. It provides various methods for generating random numbers, selecting random elements, shuffling collections and more.

## Features
- **Custom seed management**: Initialize random number generation with custom seeds.
- **Flexible random number generation**:
  * Random integers (with various range options).
  * Random doubles (uniform distribution).
  * Random booleans.
  * Gaussian distribution: Generate random numbers according to a normal distribution.
- **Advanced Utility Methods**:
  * Generate random date/time objects.
  * Select random items from collections.
  * Shuffle collections.
  * Generate random colours, GUIDs, and byte arrays.
  * Enumeration support: Easily select random values from any enumeration.
- **Thread-safe implementation**: Each thread maintains its own state for safe and efficient concurrent use, making it suitable for multi-threaded environments.

## Installation
Run `dotnet add package RandEx`

## Example
```csharp
// Import the namespace
using RandEx;
```

Basic Random Number Generation
```csharp
// Random integer between two values
int randomInt = RandomEx.GetRandomInt(10, 50);

// Random double between 0.0 and 1.0
double randomDouble = RandomEx.GetRandomDouble();

// Random boolean
bool randomBool = RandomEx.GetRandomBool();
```

Deterministic Randomization with Custom Seeds
```csharp
// Set a custom seed for deterministic random number generation
RandomEx.SetSeed(123456);
int deterministicRandom = RandomEx.GetRandomInt();
```

Gaussian Distribution
```csharp
// Generate a random number with a Gaussian (normal) distribution
double randomGaussian = RandomEx.GetRandomGaussian();
```

Random Collection Utilities
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };

// Get a random element
int randomElement = RandomEx.GetRandomElement(list);

// Shuffle the list
RandomEx.Shuffle(list);
```

Random DateTime
```csharp
// Random DateTime between two dates
DateTime randomDate = RandomEx.GetRandomDateTime(
    new DateTime(2000, 1, 1),
    new DateTime(2020, 1, 1)
);
```

Random Bytes and Colors
```csharp
// Generate a random byte array
byte[] randomBytes = RandomEx.GetRandomBytes(16);

// Generate a random color
var (R, G, B) = RandomEx.GetRandomByteColor();
```

Random Enumeration Values
```csharp
enum Colors { Red, Green, Blue, Yellow }

// Get a random enum value
Colors randomColor = RandomEx.GetRandomEnumValue<Colors>();
```

Random Char
```csharp
// Generate a char (From 'a' to 'z')
char randomChar = RandomEx.GetRandomChar('a', 'z');
// Generate a char (From '0' to '9')
char randomChar2 = RandomEx.GetRandomChar('0', '9');
// Generate a char (From ' ' to '~') default
char randomChar3 = RandomEx.GetRandomChar();
```

## API Reference

#### Random Number Methods

| Method                                | Description                                                                           |
|---------------------------------------|---------------------------------------------------------------------------------------|
| `SetSeed(int seed)`                   | Sets a custom seed for deterministic randomization.                                  |
| `GetRandomInt(int minValue = 0, int maxValue = int.MaxValue)` | Generates a random integer between `minValue` and `maxValue`.                     |
| `GetRandomDouble()`                   | Generates a random double between `0.0` and `1.0`.                                   |
| `GetRandomBool()`                     | Generates a random boolean (`true` or `false`).                                      |
| `GetRandomGaussian()`                 | Generates a random number from a Gaussian distribution (mean=0, stddev=1).          |

#### Collection Utilities

| Method                                | Description                                                                           |
|---------------------------------------|---------------------------------------------------------------------------------------|
| `GetRandomElement<T>(IList<T> collection)` | Returns a random element from the given collection.                                |
| `Shuffle<T>(List<T> values)`          | Shuffles the elements in the provided list.                                          |
| `GetRandomEnumValue<T>()`           | Returns a random value from the specified enumeration type. (e.g., Enum Colors { Red, Green }, it selects one randomly).

#### Specialized Randomization

| Method                                | Description                                                                           |
|---------------------------------------|---------------------------------------------------------------------------------------|
| `GetRandomDateTime(DateTime? minDate = null, DateTime? maxDate = null)` | Generates a random `DateTime` between the specified `minDate` and `maxDate`. If minDate is null, it defaults to DateTime.MinValue. Similarly, if maxDate is null, it defaults to DateTime.MaxValue.|
| `GetRandomByteColor()`                | Generates a random color represented as a byte tuple `(R, G, B)`.                    |
| `GetRandomGuid()`                     | Generates a random GUID.                                                             |
| `GetRandomBytes(int length)`          | Generates a random byte array of the specified length.                               |
| `GetRandomByte()`                     | Generates a random byte (0-255).                                                     |
| `GetRandomChar(char minChar = ' ', char maxChar = '~')`                     | Retrieves a random character from a specified inclusive character range, based on Unicode values.                                                     |