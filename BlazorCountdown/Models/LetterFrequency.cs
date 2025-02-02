namespace BlazorCountdown.Models
{
    public class LetterFrequency
    {
        /// <summary>
        /// Dictionary of vowels and their frequencies
        /// Based on standard English letter frequencies
        /// </summary>
        public static readonly Dictionary<char, int> VowelFrequencies = new()
        {
            { 'A', 15 }, // Common
            { 'E', 21 }, // Very common
            { 'I', 13 }, // Common
            { 'O', 13 }, // Common
            { 'U', 5 }   // Less common
        };

        /// <summary>
        /// Dictionary of consonants and their frequencies
        /// Based on standard English letter frequencies
        /// </summary>
        public static readonly Dictionary<char, int> ConsonantFrequencies = new()
        {
            { 'B', 4 },
            { 'C', 6 },
            { 'D', 8 },
            { 'F', 4 },
            { 'G', 5 },
            { 'H', 10 },
            { 'J', 1 },
            { 'K', 2 },
            { 'L', 9 },
            { 'M', 6 },
            { 'N', 13 },
            { 'P', 4 },
            { 'Q', 1 },
            { 'R', 14 },
            { 'S', 10 },
            { 'T', 15 },
            { 'V', 3 },
            { 'W', 4 },
            { 'X', 1 },
            { 'Y', 4 },
            { 'Z', 1 }
        };
    }
} 