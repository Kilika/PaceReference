using System;
using System.Linq;

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
        /// <param name="sampleSize">Maximum number of entries to include in the average.</param>
        /// <param name="gameModeId">Used GamemodeId</param>
        internal static string Build(int[] scores, int sampleSize, GameModeID gameModeId)
        {
            int usedSampleSize = Math.Min(sampleSize, scores.Length);
            if (usedSampleSize == 0)
                return NguiLabel("No scores yet!");
                
            int[] topScores = scores.Take(sampleSize).ToArray();
            int best = scores[0];
            int average = (int)topScores.Average();

            Func<int, string> formatScore = GetFormatter(gameModeId);
            return NguiLabel("Global Best:") + " " + NguiValue(formatScore(best)) + "\n" +
                   NguiLabel("Top " + topScores.Length + " Avg:") + " " + NguiValue(formatScore(average));
        }

        private static string NguiLabel(string text) => "[c][AAAAAA]" + text + "[-][/c]";
        private static string NguiValue(string text) => "[c][FFFFFF]" + text + "[-][/c]";
        private static bool IsTimeMode(GameModeID gameModeID) => gameModeID != GameModeID.Stunt;
        private static Func<int, string> GetFormatter(GameModeID gameModeId) => IsTimeMode(gameModeId) 
                ? (Func<int, string>)(ms => GUtils.GetFormattedMS(ms))
                : score => score.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + " eV";
    }
}
