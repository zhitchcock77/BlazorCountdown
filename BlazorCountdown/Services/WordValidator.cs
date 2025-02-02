using System.Text.RegularExpressions;
using BlazorCountdown.Interfaces;
using BlazorCountdown.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace BlazorCountdown.Services
{
    public class WordValidator : IWordValidator
    {
        private readonly ILogger<WordValidator> _logger;
        private HashSet<string>? _dictionary;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        public WordValidator(ILogger<WordValidator> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        private async Task EnsureDictionaryInitialized()
        {
            if (_dictionary != null) return;

            await _initializationLock.WaitAsync();
            try
            {
                if (_dictionary != null) return;
                _dictionary = await LoadDictionaryAsync();
            }
            finally
            {
                _initializationLock.Release();
            }
        }

        public async Task<bool> ValidateWordAsync(string word, string availableLetters)
        {
            await EnsureDictionaryInitialized();

            if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(availableLetters))
            {
                _logger.LogInformation("Invalid input: word or letters empty");
                return false;
            }

            word = word.ToUpperInvariant();
            availableLetters = availableLetters.ToUpperInvariant();

            // First check if it's a valid word
            if (!await IsValidWordAsync(word))
            {
                _logger.LogInformation("Word {Word} not found in dictionary", word);
                return false;
            }

            // Check if the word can be formed from the available letters
            var letterCounts = new Dictionary<char, int>();
            foreach (var letter in availableLetters)
            {
                if (!letterCounts.ContainsKey(letter))
                {
                    letterCounts[letter] = 0;
                }
                letterCounts[letter]++;
            }

            foreach (var letter in word)
            {
                if (!letterCounts.ContainsKey(letter) || letterCounts[letter] == 0)
                {
                    _logger.LogInformation("Letter {Letter} not available or depleted", letter);
                    return false;
                }
                letterCounts[letter]--;
            }

            _logger.LogInformation("Word {Word} is valid", word);
            return true;
        }

        public async Task<bool> IsValidWordAsync(string word)
        {
            await EnsureDictionaryInitialized();

            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            word = word.ToUpperInvariant();
            return _dictionary!.Contains(word);
        }

        public async Task<string[]> FindLongestPossibleWordsAsync(string letters)
        {
            await EnsureDictionaryInitialized();

            _logger.LogInformation("Finding longest possible words for letters: {Letters}", letters);

            var results = new List<string>();
            var maxLength = 0;

            foreach (var word in _dictionary!)
            {
                if (await ValidateWordAsync(word, letters))
                {
                    if (word.Length > maxLength)
                    {
                        maxLength = word.Length;
                        results.Clear();
                        results.Add(word);
                    }
                    else if (word.Length == maxLength)
                    {
                        results.Add(word);
                    }
                }
            }

            _logger.LogInformation("Found {Count} words of length {Length}", results.Count, maxLength);
            return results.ToArray();
        }

        private async Task<HashSet<string>> LoadDictionaryAsync()
        {
            try
            {
                var dictionary = new HashSet<string>();
                var response = await _httpClient.GetStringAsync("dictionary.txt");
                var lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var word = line.Trim().ToUpperInvariant();
                    
                    // Only add words that:
                    // 1. Are between 3 and 9 letters long
                    // 2. Contain only letters (no numbers, hyphens, etc.)
                    // 3. Don't contain repeated letters more than what's available in the game
                    // 4. Are not proper nouns (don't start with uppercase in original)
                    if (word.Length >= 3 && 
                        word.Length <= GameConstants.TotalLettersPerRound && 
                        Regex.IsMatch(word, "^[A-Z]+$") &&
                        !HasTooManyRepeatedLetters(word) &&
                        char.IsLower(line.Trim()[0]))
                    {
                        dictionary.Add(word);
                    }
                }

                _logger.LogInformation("Loaded {Count} words from dictionary file", dictionary.Count);
                return dictionary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dictionary file");
                return LoadFallbackDictionary();
            }
        }

        private HashSet<string> LoadFallbackDictionary()
        {
            _logger.LogWarning("Using fallback dictionary");
            return new HashSet<string>
            {
                "CAT", "DOG", "BATH", "HOUSE", "COMPUTER",
                "THE", "AND", "THAT", "HAVE", "FOR",
                "NOT", "WITH", "YOU", "THIS", "BUT",
                "HIS", "FROM", "THEY", "SAY", "HER",
                "SHE", "WILL", "ONE", "ALL", "WOULD",
                "THERE", "THEIR", "WHAT", "OUT", "ABOUT",
                "WHO", "GET", "WHICH", "WHEN", "MAKE",
                "CAN", "LIKE", "TIME", "JUST", "HIM",
                "KNOW", "TAKE", "PEOPLE", "INTO", "YEAR",
                "YOUR", "GOOD", "SOME", "COULD", "THEM"
            };
        }

        private bool HasTooManyRepeatedLetters(string word)
        {
            var letterCounts = new Dictionary<char, int>();
            foreach (var letter in word)
            {
                if (!letterCounts.ContainsKey(letter))
                {
                    letterCounts[letter] = 0;
                }
                letterCounts[letter]++;

                // If any letter appears more than 2 times, it's unlikely to be useful in the game
                if (letterCounts[letter] > 2)
                {
                    return true;
                }
            }
            return false;
        }
    }
} 