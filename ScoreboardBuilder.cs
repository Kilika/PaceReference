using System;

namespace PaceRef
{
    internal static class ScoreboardBuilder
    {
        /// <summary>
        /// Builds the NGUI-formatted scoreboard string.
        /// </summary>
        /// <param name="scores">
        /// Pre-sorted scores from the leaderboard.
        /// For time modes: ascending (lowest ms first = best).
        /// For points modes (Stunt): descending (highest eV first = best).
        /// </param>
        /// <param name="isTimeMode">
        /// False for Stunt (scores are raw points); true for all other modes (scores are milliseconds).
        /// </param>
        /// <param name="sampleSize">Maximum number of entries to include in the average.</param>
        /// <param name="formatScore">Converts a raw score int to its display string.</param>
        internal static string Build(int[] scores, bool isTimeMode, int sampleSize, Func<int, string> formatScore)
        {
            if (scores == null || scores.Length == 0)
                return "No leaderboard data";

            int usedSampleSize = scores.Length < sampleSize ? scores.Length : sampleSize;
            int best = scores[0];

            long sum = 0;
            for (int i = 0; i < usedSampleSize; i++)
                sum += scores[i];
            int average = (int)(sum / usedSampleSize);

           return "[c][AAAAAA]" + "Global Best" + ":[-][/c] [c][FFFFFF]" + formatScore(best) + "[-][/c]\n" +
                   "[c][AAAAAA]Top " + usedSampleSize + " Avg:[-][/c] [c][FFFFFF]" + formatScore(average) + "[-][/c]";
        }

        internal static bool IsTimeMode(GameModeID gameModeID) => gameModeID != GameModeID.Stunt;

        internal static Func<int, string> GetFormatter(bool isTimeMode)
        {
            return isTimeMode
                ? (Func<int, string>)(ms => GUtils.GetFormattedMS(ms))
                : score => score.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + " eV";
        }
    }
}
