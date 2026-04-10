using System;
using NUnit.Framework;

namespace PaceRef.Tests
{
    [TestFixture]
    internal class ScoreboardBuilderTests
    {
        [Test]
        public void Build_EmptyScores_ReturnsEmpty()
        {
            string result = ScoreboardBuilder.Build(new int[0], 10, GameModeID.Sprint);
            StringAssert.Contains("No scores yet!", result);
        }

        [Test]
        public void Build_SprintMode_ContainsGlobalBestLabel()
        {
            int[] scores = { 30000, 31000, 32000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Sprint);
            StringAssert.Contains("Global Best:", result);
        }

        [Test]
        public void Build_SprintMode_BestTime_IsFirstEntry()
        {
            int[] scores = { 30000, 31000, 32000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Sprint);
            StringAssert.Contains(GUtils.GetFormattedMS(30000), result);
        }

        [Test]
        public void Build_SprintMode_Average_UsesAllEntriesWhenSampleLarger()
        {
            int[] scores = { 30000, 32000, 34000 }; // avg = 32 000 ms
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Sprint);
            StringAssert.Contains(GUtils.GetFormattedMS(32000), result);
        }

        [Test]
        public void Build_SprintMode_Average_LimitedBySampleSize()
        {
            // 5 entries, sampleSize=3 → average of first 3 only = 20 000 ms
            int[] scores = { 10000, 20000, 30000, 100000, 200000 };
            string result = ScoreboardBuilder.Build(scores, 3, GameModeID.Sprint);
            StringAssert.Contains(GUtils.GetFormattedMS(20000), result);
            StringAssert.Contains("Top 3 Avg", result);
        }

        [Test]
        public void Build_SprintMode_SampleSizeLabelMatchesUsedCount()
        {
            // 2 entries, sampleSize=100 → "Top 2 Avg"
            int[] scores = { 5000, 7000 };
            string result = ScoreboardBuilder.Build(scores, 100, GameModeID.Sprint);
            StringAssert.Contains("Top 2 Avg", result);
        }

        [Test]
        public void Build_SprintMode_SingleEntry_BestEqualsAverage()
        {
            int[] scores = { 45678 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Sprint);
            int occurrences = CountOccurrences(result, GUtils.GetFormattedMS(45678));
            Assert.That(occurrences, Is.EqualTo(2));
        }

        [Test]
        public void Build_StuntMode_ContainsGlobalBestLabel()
        {
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Stunt);
            StringAssert.Contains("Global Best:", result);
        }

        [Test]
        public void Build_StuntMode_BestScore_ContainsEvUnit()
        {
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Stunt);
            StringAssert.Contains("9,000 eV", result);
        }

        [Test]
        public void Build_StuntMode_Average_ContainsEvUnit()
        {
            // avg of {9000, 7500, 6000} = 7500
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Stunt);
            StringAssert.Contains("7,500 eV", result);
        }

        [Test]
        public void Build_StuntMode_BestScore_IsFirstEntry()
        {
            int[] scores = { 12345, 10000, 8000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Stunt);
            StringAssert.Contains("12,345 eV", result);
        }

        [Test]
        public void Build_StuntMode_Average_LimitedBySampleSize()
        {
            // sampleSize=2 → average of {9000, 7500} = 8250
            int[] scores = { 9000, 7500, 6000, 1000 };
            string result = ScoreboardBuilder.Build(scores, 2, GameModeID.Stunt);
            StringAssert.Contains("8,250 eV", result);
            StringAssert.Contains("Top 2 Avg", result);
        }

        [Test]
        public void Build_StuntMode_ScoreValues_ContainEvNotTimeFormat()
        {
            int[] scores = { 90000, 60000 };
            string result = ScoreboardBuilder.Build(scores, 10, GameModeID.Stunt);
            StringAssert.Contains("90,000 eV", result);
            StringAssert.Contains("75,000 eV", result);
            StringAssert.DoesNotContain("1:30", result);
        }

        private static int CountOccurrences(string text, string pattern)
        {
            int count = 0, idx = 0;
            while ((idx = text.IndexOf(pattern, idx, StringComparison.Ordinal)) >= 0)
            {
                count++;
                idx += pattern.Length;
            }
            return count;
        }
    }
}
