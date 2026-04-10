using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;

namespace PaceRef.Patches
{
    [HarmonyPatch(typeof(PauseMenuLogic), nameof(PauseMenuLogic.SetupPauseMenuLevelInfo))]
    internal class PauseMenuLogic_AddDetailedScoreBoardPanel
    {
        private static ManualLogSource Log => Mod.Logger;

        [HarmonyPostfix]
        public static void SetupPauseMenuLevelInfo_Postfix(PauseMenuLogic __instance)
        {
            string levelPath = G.Sys.GameManager_.LevelPath_;
            GameModeID gameModeID = G.Sys.GameManager_.Mode_.GameModeID_;

            bool isTimeMode = ScoreboardBuilder.IsTimeMode(gameModeID);
            Func<int, string> formatScore = ScoreboardBuilder.GetFormatter(isTimeMode);

            G.Sys.SteamworksManager_.Leaderboard_.DownloadLeaderboardInfo(
                levelPath,
                gameModeID,
                onLeaderboardDownloaded: (success, entries, count) =>
                {
                    if (entries == null || entries.Length <= 0)
                        return;

                    int[] scores = Enumerable.Select(entries, e => e.Score_).ToArray();
                    string scoreboardData = ScoreboardBuilder.Build(scores, isTimeMode, Mod.SampleSizeConfig.Value, formatScore);
                    AddScoreboardDataToLabel(__instance, scoreboardData);
                },
                OnlineLeaderboard.RangeRequestType.Global, 1, Mod.SampleSizeConfig.Value
            );
        }

        private static void AddScoreboardDataToLabel(PauseMenuLogic pauseMenuLogic, string scoreboardData)
        {
            Log.LogInfo("=== Adding scoreboard data to existing label ===");

            pauseMenuLogic.levelInfoLabel_.text += $"\n\n{scoreboardData}";
            pauseMenuLogic.levelInfoLabel_.overflowMethod = UILabel.Overflow.ResizeHeight;
            pauseMenuLogic.levelInfoLabel_.MakePixelPerfect();

            Log.LogInfo("=== Scoreboard data added successfully ===");
        }
    }
}
