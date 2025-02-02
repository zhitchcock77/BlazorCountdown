using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorCountdown.Interfaces
{
    public interface ILetterProvider
    {
        /// <summary>
        /// Gets a random consonant based on English letter frequency
        /// </summary>
        /// <returns>A consonant character</returns>
        Task<char> GetConsonantAsync();

        /// <summary>
        /// Gets a random vowel based on English letter frequency
        /// </summary>
        /// <returns>A vowel character</returns>
        Task<char> GetVowelAsync();

        /// <summary>
        /// Checks if more consonants are available
        /// </summary>
        /// <returns>True if consonants are available, false otherwise</returns>
        bool HasConsonantsAvailable();

        /// <summary>
        /// Checks if more vowels are available
        /// </summary>
        /// <returns>True if vowels are available, false otherwise</returns>
        bool HasVowelsAvailable();

        /// <summary>
        /// Resets the letter pools to their initial state
        /// </summary>
        void ResetLetterPools();

        /// <summary>
        /// Gets the remaining count of consonants
        /// </summary>
        /// <returns>Number of consonants remaining</returns>
        int GetRemainingConsonants();

        /// <summary>
        /// Gets the remaining count of vowels
        /// </summary>
        /// <returns>Number of vowels remaining</returns>
        int GetRemainingVowels();
    }
} 