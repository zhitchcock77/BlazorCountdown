using System.Text.RegularExpressions;
using BlazorCountdown.Interfaces;
using BlazorCountdown.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace BlazorCountdown.Services
{
    public class WordValidator : IWordValidator
    {
        private readonly ILogger<WordValidator> _logger;
        private HashSet<string>? _dictionary;
        private Dictionary<int, HashSet<string>>? _wordsByLength;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        public WordValidator(ILogger<WordValidator> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task EnsureDictionaryInitialized()
        {
            if (_dictionary != null) return;

            await _initializationLock.WaitAsync();
            try
            {
                if (_dictionary != null) return;
                _dictionary = await LoadDictionaryAsync();
                _logger.LogInformation("Dictionary initialized with {Count} words", _dictionary.Count);
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

            // Create letter frequency map once
            var letterCounts = new Dictionary<char, int>();
            foreach (var letter in letters.ToUpperInvariant())
            {
                if (!letterCounts.ContainsKey(letter))
                    letterCounts[letter] = 0;
                letterCounts[letter]++;
            }

            var results = new List<string>();
            
            // Start from the longest possible length (9) and work backwards
            for (int length = GameConstants.TotalLettersPerRound; length >= 3; length--)
            {
                if (_wordsByLength!.TryGetValue(length, out var wordsOfLength))
                {
                    foreach (var word in wordsOfLength)
                    {
                        if (CanFormWord(word, new Dictionary<char, int>(letterCounts)))
                        {
                            results.Add(word);
                        }
                    }

                    // If we found any words of this length, we're done
                    if (results.Count > 0)
                        break;
                }
            }

            _logger.LogInformation("Found {Count} words of length {Length}", 
                results.Count, 
                results.Any() ? results[0].Length : 0);
                
            return results.ToArray();
        }

        private bool CanFormWord(string word, Dictionary<char, int> availableLetters)
        {
            foreach (var letter in word)
            {
                if (!availableLetters.ContainsKey(letter) || availableLetters[letter] == 0)
                    return false;
                availableLetters[letter]--;
            }
            return true;
        }

        private async Task<HashSet<string>> LoadDictionaryAsync()
        {
            _logger.LogInformation("Loading optimized dictionary...");
            var dictionary = new HashSet<string>();
            _wordsByLength = new Dictionary<int, HashSet<string>>();

            var optimizedResponse = await _httpClient.GetStringAsync("optimized_dictionary.json");
            var loadedDictionary = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(optimizedResponse);
            
            if (loadedDictionary == null)
            {
                throw new InvalidOperationException("Failed to deserialize optimized dictionary");
            }

            _logger.LogInformation("Loading from optimized dictionary file");
            foreach (var kvp in loadedDictionary)
            {
                if (int.TryParse(kvp.Key, out int length))
                {
                    _wordsByLength[length] = kvp.Value;
                    dictionary.UnionWith(kvp.Value);
                }
            }

            _logger.LogInformation("Dictionary loaded. Total words: {Count}", dictionary.Count);
            foreach (var bucket in _wordsByLength)
            {
                _logger.LogInformation("{Length}-letter words: {Count}", bucket.Key, bucket.Value.Count);
            }

            return dictionary;
        }
    }
} 