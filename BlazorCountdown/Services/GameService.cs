using BlazorCountdown.Interfaces;
using BlazorCountdown.Models;
using Microsoft.Extensions.Logging;

namespace BlazorCountdown.Services
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> _logger;
        private readonly ILetterProvider _letterProvider;
        private readonly IWordValidator _wordValidator;
        private GameState _currentState;

        public GameService(
            ILogger<GameService> logger,
            ILetterProvider letterProvider,
            IWordValidator wordValidator)
        {
            _logger = logger;
            _letterProvider = letterProvider;
            _wordValidator = wordValidator;
            _currentState = new GameState();
        }

        public bool IsRoundComplete => _currentState.IsRoundComplete;
        public bool IsGameComplete => _currentState.IsGameComplete;
        public int CurrentRound => _currentState.CurrentRound;
        public int TotalScore => _currentState.TotalScore;

        public async Task StartNewGameAsync()
        {
            _logger.LogInformation("Starting new game");
            
            await Task.Run(() => {
                _letterProvider.ResetLetterPools();
                _currentState = new GameState
                {
                    CurrentRound = 1,
                    IsLetterSelectionPhase = true,
                    RemainingConsonantSelections = GameConstants.MaxConsonants,
                    RemainingVowelSelections = GameConstants.MaxVowels,
                    RemainingTime = GameConstants.RoundTimeLimit
                };
            });
        }

        public async Task<char> RequestConsonantAsync()
        {
            if (!_currentState.IsLetterSelectionPhase)
            {
                throw new InvalidOperationException("Not in letter selection phase");
            }

            if (_currentState.RemainingConsonantSelections <= 0)
            {
                _logger.LogWarning("No more consonants can be selected this round");
                return '\0';
            }

            if (_currentState.SelectedLetters.Count >= GameConstants.TotalLettersPerRound)
            {
                _logger.LogWarning("Maximum letters already selected");
                return '\0';
            }

            try
            {
                var consonant = await _letterProvider.GetConsonantAsync();
                _currentState.SelectedLetters.Add(consonant);
                _currentState.RemainingConsonantSelections--;

                // Check if we've selected all letters
                if (_currentState.SelectedLetters.Count >= GameConstants.TotalLettersPerRound)
                {
                    _currentState.IsLetterSelectionPhase = false;
                    _currentState.IsWordFormationPhase = true;
                }

                _logger.LogInformation("Selected consonant: {Consonant}", consonant);
                return consonant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting consonant");
                return '\0';
            }
        }

        public async Task<char> RequestVowelAsync()
        {
            if (!_currentState.IsLetterSelectionPhase)
            {
                throw new InvalidOperationException("Not in letter selection phase");
            }

            if (_currentState.RemainingVowelSelections <= 0)
            {
                _logger.LogWarning("No more vowels can be selected this round");
                return '\0';
            }

            if (_currentState.SelectedLetters.Count >= GameConstants.TotalLettersPerRound)
            {
                _logger.LogWarning("Maximum letters already selected");
                return '\0';
            }

            try
            {
                var vowel = await _letterProvider.GetVowelAsync();
                _currentState.SelectedLetters.Add(vowel);
                _currentState.RemainingVowelSelections--;

                // Check if we've selected all letters
                if (_currentState.SelectedLetters.Count >= GameConstants.TotalLettersPerRound)
                {
                    _currentState.IsLetterSelectionPhase = false;
                    _currentState.IsWordFormationPhase = true;
                }

                _logger.LogInformation("Selected vowel: {Vowel}", vowel);
                return vowel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting vowel");
                return '\0';
            }
        }

        public async Task<int> SubmitWordAsync(string word)
        {
            if (!_currentState.IsWordFormationPhase)
            {
                throw new InvalidOperationException("Not in word formation phase");
            }

            var availableLetters = new string(_currentState.SelectedLetters.ToArray());
            var isValid = await _wordValidator.ValidateWordAsync(word, availableLetters);

            int score = 0;
            if (isValid)
            {
                score = word.Length;
                if (word.Length == GameConstants.TotalLettersPerRound)
                {
                    score *= GameConstants.NineLetterWordMultiplier;
                }
            }

            // Find the longest possible words that could have been formed
            _currentState.LongestPossibleWords = await _wordValidator.FindLongestPossibleWordsAsync(availableLetters);
            
            _currentState.SubmittedWord = word;
            _currentState.CurrentRoundScore = score;
            _currentState.TotalScore += score;
            _currentState.IsWordFormationPhase = false;
            _currentState.IsRoundComplete = true;

            _logger.LogInformation("Word submitted: {Word}, Score: {Score}", word, score);
            return score;
        }

        public async Task<bool> StartNextRoundAsync()
        {
            if (!_currentState.IsRoundComplete)
            {
                throw new InvalidOperationException("Current round is not complete");
            }

            if (_currentState.CurrentRound >= GameConstants.TotalRounds)
            {
                _currentState.IsGameComplete = true;
                return false;
            }

            await Task.Run(() => {
                _letterProvider.ResetLetterPools();
                _currentState.CurrentRound++;
                _currentState.IsLetterSelectionPhase = true;
                _currentState.IsWordFormationPhase = false;
                _currentState.IsRoundComplete = false;
                _currentState.SelectedLetters.Clear();
                _currentState.SubmittedWord = string.Empty;
                _currentState.LongestPossibleWords = Array.Empty<string>();
                _currentState.RemainingConsonantSelections = GameConstants.MaxConsonants;
                _currentState.RemainingVowelSelections = GameConstants.MaxVowels;
                _currentState.RemainingTime = GameConstants.RoundTimeLimit;
            });

            _logger.LogInformation("Starting round {Round}", _currentState.CurrentRound);
            return true;
        }

        public Task<GameState> GetGameStateAsync()
        {
            return Task.FromResult(_currentState);
        }
    }
} 