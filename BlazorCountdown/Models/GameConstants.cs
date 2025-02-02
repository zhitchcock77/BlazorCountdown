namespace BlazorCountdown.Models
{
    public static class GameConstants
    {
        /// <summary>
        /// The total number of rounds in a game
        /// </summary>
        public const int TotalRounds = 4;

        /// <summary>
        /// The time limit for each round in seconds
        /// </summary>
        public const int RoundTimeLimit = 30;

        /// <summary>
        /// The total number of letters to select in each round
        /// </summary>
        public const int TotalLettersPerRound = 9;

        /// <summary>
        /// The minimum number of vowels required per round
        /// </summary>
        public const int MinVowels = 3;

        /// <summary>
        /// The maximum number of vowels allowed per round
        /// </summary>
        public const int MaxVowels = 5;

        /// <summary>
        /// The minimum number of consonants required per round
        /// </summary>
        public const int MinConsonants = 4;

        /// <summary>
        /// The maximum number of consonants allowed per round
        /// </summary>
        public const int MaxConsonants = 6;

        /// <summary>
        /// The score multiplier for finding a 9-letter word
        /// </summary>
        public const int NineLetterWordMultiplier = 2;
    }
} 