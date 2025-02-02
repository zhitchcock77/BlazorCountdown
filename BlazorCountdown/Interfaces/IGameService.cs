using System.Threading.Tasks;
using BlazorCountdown.Models;

namespace BlazorCountdown.Interfaces
{
    public interface IGameService
    {
        /// <summary>
        /// Starts a new game
        /// </summary>
        Task StartNewGameAsync();

        /// <summary>
        /// Requests a consonant for the current round
        /// </summary>
        /// <returns>The selected consonant</returns>
        Task<char> RequestConsonantAsync();

        /// <summary>
        /// Requests a vowel for the current round
        /// </summary>
        /// <returns>The selected vowel</returns>
        Task<char> RequestVowelAsync();

        /// <summary>
        /// Submits a word for the current round
        /// </summary>
        /// <param name="word">The word to submit</param>
        /// <returns>The score for the submitted word</returns>
        Task<int> SubmitWordAsync(string word);

        /// <summary>
        /// Starts the next round
        /// </summary>
        /// <returns>True if there is a next round, false if game is complete</returns>
        Task<bool> StartNextRoundAsync();

        /// <summary>
        /// Gets the current game state
        /// </summary>
        /// <returns>The current game state</returns>
        Task<GameState> GetGameStateAsync();

        /// <summary>
        /// Gets whether the current round is complete
        /// </summary>
        bool IsRoundComplete { get; }

        /// <summary>
        /// Gets whether the game is complete
        /// </summary>
        bool IsGameComplete { get; }

        /// <summary>
        /// Gets the current round number (1-4)
        /// </summary>
        int CurrentRound { get; }

        /// <summary>
        /// Gets the total score across all rounds
        /// </summary>
        int TotalScore { get; }
    }
} 