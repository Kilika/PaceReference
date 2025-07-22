using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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
            
            G.Sys.SteamworksManager_.Leaderboard_.DownloadLeaderboardInfo(
                levelPath, 
                gameModeID,
                onLeaderboardDownloaded: (success, entries, count) =>
                {
                    if (entries == null || entries.Length <= 0)
                        return; 
                    
                    AddScoreboardDataToLabel(__instance, entries);
                },
                OnlineLeaderboard.RangeRequestType.Global, 1, Mod.SampleSizeConfig.Value
            );
        }

        private static void AddScoreboardDataToLabel(PauseMenuLogic pauseMenuLogic, SteamworksLeaderboard.Entry[] entries)
        {
            Log.LogInfo("=== Adding scoreboard data to existing label ===");
            
            string scoreboardData = BuildScoreboardText(entries);
            
            pauseMenuLogic.levelInfoLabel_.text += $"\n\n{scoreboardData}";
            pauseMenuLogic.levelInfoLabel_.overflowMethod = UILabel.Overflow.ResizeHeight;
            pauseMenuLogic.levelInfoLabel_.MakePixelPerfect();
            
            Log.LogInfo("=== Scoreboard data added successfully ===");
        }

        private static string BuildScoreboardText(SteamworksLeaderboard.Entry[] entries)
        {
            if (entries == null || !entries.Any())
                return "No leaderboard data";
            
            int configSampleSize = Mod.SampleSizeConfig.Value;
            int usedSampleSize = entries.Length < configSampleSize ? entries.Length : configSampleSize;
            Log.LogInfo($"Using sample size: {usedSampleSize} out of {entries.Length} total entries.");
            
            int globalBest = entries[0].Score_;
            string globalBestText = GUtils.GetFormattedMS(globalBest);

            int averagePro = entries.Take(usedSampleSize).Sum(x => x.Score_) / usedSampleSize;
            string averageProText = GUtils.GetFormattedMS(averagePro);

            return $"[c][AAAAAA]Global Best:[-][/c] [c][FFFFFF]{globalBestText}[-][/c]\n" +
                   $"[c][AAAAAA]Top {usedSampleSize} Avg:[-][/c] [c][FFFFFF]{averageProText}[-][/c]";
        }
    }
}