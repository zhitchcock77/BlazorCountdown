using System.Collections.Generic;

namespace BlazorCountdown.Models
{
    public class GameState
    {
        /// <summary>
        /// The current round number (1-4)
        /// </summary>
        public int CurrentRound { get; set; } = 1;

        /// <summary>
        /// The selected letters for the current round
        /// </summary>
        public List<char> SelectedLetters { get; set; } = new();

        /// <summary>
        /// The remaining time in seconds for the current round
        /// </summary>
        public int RemainingTime { get; set; }

        /// <summary>
        /// The score for the current round
        /// </summary>
        public int CurrentRoundScore { get; set; }

        /// <summary>
        /// The total score across all rounds
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// The submitted word for the current round
        /// </summary>
        public string SubmittedWord { get; set; } = string.Empty;

        /// <summary>
        /// The longest possible words that could have been formed
        /// </summary>
        public string[] LongestPossibleWords { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Whether the current round is in the letter selection phase
        /// </summary>
        public bool IsLetterSelectionPhase { get; set; }

        /// <summary>
        /// Whether the current round is in the word formation phase
        /// </summary>
        public bool IsWordFormationPhase { get; set; }

        /// <summary>
        /// Whether the current round is complete
        /// </summary>
        public bool IsRoundComplete { get; set; }

        /// <summary>
        /// Whether the game is complete
        /// </summary>
        public bool IsGameComplete { get; set; }

        /// <summary>
        /// The number of consonants that can still be selected
        /// </summary>
        public int RemainingConsonantSelections { get; set; }

        /// <summary>
        /// The number of vowels that can still be selected
        /// </summary>
        public int RemainingVowelSelections { get; set; }
    }
} 