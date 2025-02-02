using BlazorCountdown.Interfaces;
using BlazorCountdown.Models;
using Microsoft.Extensions.Logging;

namespace BlazorCountdown.Services
{
    public class LetterProvider : ILetterProvider
    {
        private readonly ILogger<LetterProvider> _logger;
        private readonly Random _random;
        private Dictionary<char, int> _remainingVowels;
        private Dictionary<char, int> _remainingConsonants;

        public LetterProvider(ILogger<LetterProvider> logger)
        {
            _logger = logger;
            _random = new Random();
            _remainingVowels = new Dictionary<char, int>();
            _remainingConsonants = new Dictionary<char, int>();
            ResetLetterPools();
        }

        public Task<char> GetConsonantAsync()
        {
            _logger.LogInformation("Getting consonant");
            
            if (!HasConsonantsAvailable())
            {
                throw new InvalidOperationException("No consonants available");
            }

            var totalWeight = _remainingConsonants.Sum(x => x.Value);
            var randomNumber = _random.Next(totalWeight);
            var currentWeight = 0;

            foreach (var kvp in _remainingConsonants.ToList())
            {
                currentWeight += kvp.Value;
                if (randomNumber < currentWeight)
                {
                    // Decrease the count of this consonant
                    _remainingConsonants[kvp.Key]--;
                    if (_remainingConsonants[kvp.Key] == 0)
                    {
                        _remainingConsonants.Remove(kvp.Key);
                    }

                    _logger.LogInformation("Selected consonant: {Consonant}", kvp.Key);
                    return Task.FromResult(kvp.Key);
                }
            }

            // This should never happen if the math is correct
            throw new InvalidOperationException("Failed to select a consonant");
        }

        public Task<char> GetVowelAsync()
        {
            _logger.LogInformation("Getting vowel");
            
            if (!HasVowelsAvailable())
            {
                throw new InvalidOperationException("No vowels available");
            }

            var totalWeight = _remainingVowels.Sum(x => x.Value);
            var randomNumber = _random.Next(totalWeight);
            var currentWeight = 0;

            foreach (var kvp in _remainingVowels.ToList())
            {
                currentWeight += kvp.Value;
                if (randomNumber < currentWeight)
                {
                    // Decrease the count of this vowel
                    _remainingVowels[kvp.Key]--;
                    if (_remainingVowels[kvp.Key] == 0)
                    {
                        _remainingVowels.Remove(kvp.Key);
                    }

                    _logger.LogInformation("Selected vowel: {Vowel}", kvp.Key);
                    return Task.FromResult(kvp.Key);
                }
            }

            // This should never happen if the math is correct
            throw new InvalidOperationException("Failed to select a vowel");
        }

        public bool HasConsonantsAvailable() => _remainingConsonants.Any(x => x.Value > 0);

        public bool HasVowelsAvailable() => _remainingVowels.Any(x => x.Value > 0);

        public void ResetLetterPools()
        {
            _logger.LogInformation("Resetting letter pools");
            
            // Create new dictionaries with initial frequencies
            _remainingVowels = new Dictionary<char, int>(LetterFrequency.VowelFrequencies);
            _remainingConsonants = new Dictionary<char, int>(LetterFrequency.ConsonantFrequencies);
        }

        public int GetRemainingConsonants() => _remainingConsonants.Sum(x => x.Value);

        public int GetRemainingVowels() => _remainingVowels.Sum(x => x.Value);
    }
} 