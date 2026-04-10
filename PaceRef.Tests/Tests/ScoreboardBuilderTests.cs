using System;
using NUnit.Framework;

namespace PaceRef.Tests
{
    [TestFixture]
    internal class ScoreboardBuilderTests
    {
        /// Simple time formatter for test assertions (does not depend on Unity).
        private static readonly Func<int, string> TimeFormat = ms => GUtils.GetFormattedMS(ms);

        /// Points formatter used by Stunt mode.
        private static readonly Func<int, string> PointsFormat = score => $"{score} eV";

        [Test]
        public void Build_NullScores_ReturnsNoDataString()
        {
            string result = ScoreboardBuilder.Build(null, isTimeMode: true, sampleSize: 10, TimeFormat);
            Assert.That(result, Is.EqualTo("No leaderboard data"));
        }

        [Test]
        public void Build_EmptyScores_ReturnsNoDataString()
        {
            string result = ScoreboardBuilder.Build(new int[0], isTimeMode: true, sampleSize: 10, TimeFormat);
            Assert.That(result, Is.EqualTo("No leaderboard data"));
        }

        // ── Sprint / time mode ────────────────────────────────────────────────────

        [Test]
        public void Build_SprintMode_BestLabel_IsGlobalBest()
        {
            // Sprint scores: ascending (lowest time first = best)
            int[] scores = { 30000, 31000, 32000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 10, TimeFormat);
            StringAssert.Contains("Global Best:", result);
        }

        [Test]
        public void Build_SprintMode_DoesNotContainScoreLabel()
        {
            int[] scores = { 30000, 31000, 32000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 10, TimeFormat);
            StringAssert.DoesNotContain("Global Best Score:", result);
        }

        [Test]
        public void Build_SprintMode_BestTime_IsFirstEntry()
        {
            // scores[0] = 30 000 ms = 0:30.000
            int[] scores = { 30000, 31000, 32000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 10, TimeFormat);
            StringAssert.Contains(GUtils.GetFormattedMS(30000), result);
        }

        [Test]
        public void Build_SprintMode_Average_UsesAllEntriesWhenSampleLarger()
        {
            // 3 entries, sampleSize=10 → average of all three
            int[] scores = { 30000, 32000, 34000 }; // avg = 32 000 ms
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 10, TimeFormat);
            StringAssert.Contains(GUtils.GetFormattedMS(32000), result);
        }

        [Test]
        public void Build_SprintMode_Average_LimitedBySampleSize()
        {
            // 5 entries, sampleSize=3 → average of first 3 only
            int[] scores = { 10000, 20000, 30000, 100000, 200000 }; // avg of first 3 = 20 000
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 3, TimeFormat);
            StringAssert.Contains(GUtils.GetFormattedMS(20000), result);
            StringAssert.Contains("Top 3 Avg", result);
        }

        [Test]
        public void Build_SprintMode_SampleSizeLabelMatchesUsedCount()
        {
            // 2 entries, sampleSize=100 → "Top 2 Avg"
            int[] scores = { 5000, 7000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 100, TimeFormat);
            StringAssert.Contains("Top 2 Avg", result);
        }

        [Test]
        public void Build_SprintMode_SingleEntry_BestEqualsAverage()
        {
            int[] scores = { 45678 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: true, sampleSize: 10, TimeFormat);
            // Both global best and average should show the same value
            int occurrences = CountOccurrences(result, GUtils.GetFormattedMS(45678));
            Assert.That(occurrences, Is.EqualTo(2));
        }

        // ── Stunt / points mode ───────────────────────────────────────────────────

        [Test]
        public void Build_StuntMode_BestLabel_IsGlobalBestScore()
        {
            // Stunt scores: descending (highest eV first = best)
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            StringAssert.Contains("Global Best Score:", result);
        }

        [Test]
        public void Build_StuntMode_DoesNotContainPlainGlobalBestLabel()
        {
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            // "Global Best Score:" is present but "Global Best:" (without "Score") must not appear
            // standalone — guard against the time-mode label being used
            Assert.That(result, Does.Not.Contain("[c][AAAAAA]Global Best:[-]"));
        }

        [Test]
        public void Build_StuntMode_BestScore_ContainsEvUnit()
        {
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            StringAssert.Contains("9000 eV", result);
        }

        [Test]
        public void Build_StuntMode_Average_ContainsEvUnit()
        {
            // avg of {9000, 7500, 6000} = 7500
            int[] scores = { 9000, 7500, 6000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            StringAssert.Contains("7500 eV", result);
        }

        [Test]
        public void Build_StuntMode_BestScore_IsFirstEntry()
        {
            // Highest eV is first (Steam returns descending for Stunt)
            int[] scores = { 12345, 10000, 8000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            StringAssert.Contains("12345 eV", result);
        }

        [Test]
        public void Build_StuntMode_Average_LimitedBySampleSize()
        {
            // sampleSize=2 → average of {9000, 7500} = 8250
            int[] scores = { 9000, 7500, 6000, 1000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 2, PointsFormat);
            StringAssert.Contains("8250 eV", result);
            StringAssert.Contains("Top 2 Avg", result);
        }

        [Test]
        public void Build_StuntMode_ScoreValues_ContainEvNotTimeFormat()
        {
            int[] scores = { 90000, 60000 };
            string result = ScoreboardBuilder.Build(scores, isTimeMode: false, sampleSize: 10, PointsFormat);
            // Stunt scores must appear as "90000 eV" / "75000 eV", not as "1:30.000" etc.
            StringAssert.Contains("90000 eV", result);
            StringAssert.Contains("75000 eV", result);
            StringAssert.DoesNotContain("1:30", result);
        }

        // ── IsTimeMode helper ─────────────────────────────────────────────────────

        [TestCase(GameModeID.Sprint,     true)]
        [TestCase(GameModeID.CoopSprint, true)]
        [TestCase(GameModeID.Challenge,  true)]
        [TestCase(GameModeID.Stunt,      false)]
        public void IsTimeMode_ReturnsExpected(GameModeID mode, bool expected)
        {
            Assert.That(ScoreboardBuilder.IsTimeMode(mode), Is.EqualTo(expected));
        }

        // ── GetFormatter helper ───────────────────────────────────────────────────

        [Test]
        public void GetFormatter_TimeMode_FormatsMilliseconds()
        {
            Func<int, string> fmt = ScoreboardBuilder.GetFormatter(isTimeMode: true);
            Assert.That(fmt(60000), Is.EqualTo("1:00.000"));
        }

        [Test]
        public void GetFormatter_PointsMode_AppendsEv()
        {
            Func<int, string> fmt = ScoreboardBuilder.GetFormatter(isTimeMode: false);
            Assert.That(fmt(4200), Is.EqualTo("4.200 eV"));
        }

        // ── helpers ───────────────────────────────────────────────────────────────

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
