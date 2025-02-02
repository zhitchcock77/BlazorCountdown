using System.Threading.Tasks;

namespace BlazorCountdown.Interfaces
{
    public interface IWordValidator
    {
        /// <summary>
        /// Ensures the dictionary is initialized and optimized
        /// </summary>
        Task EnsureDictionaryInitialized();

        /// <summary>
        /// Validates if a word can be formed from the given letters and exists in the dictionary
        /// </summary>
        /// <param name="word">The word to validate</param>
        /// <param name="availableLetters">The letters available to form the word</param>
        /// <returns>True if the word is valid, false otherwise</returns>
        Task<bool> ValidateWordAsync(string word, string availableLetters);

        /// <summary>
        /// Checks if a word exists in the dictionary
        /// </summary>
        /// <param name="word">The word to check</param>
        /// <returns>True if the word exists in the dictionary, false otherwise</returns>
        Task<bool> IsValidWordAsync(string word);

        /// <summary>
        /// Finds the longest possible word(s) that can be formed from the given letters
        /// </summary>
        /// <param name="letters">The available letters</param>
        /// <returns>Array of longest possible words</returns>
        Task<string[]> FindLongestPossibleWordsAsync(string letters);
    }
} 