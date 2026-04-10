using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;

namespace PaceRef.Patches
{
    [HarmonyPatch(typeof(PauseMenuLogic), nameof(PauseMenuLogic.SetupPauseMenuLevelInfo))]
    internal class PauseMenuLogic_AddDetailedScoreBoardPanel
    {
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
            var existing = pauseMenuLogic.levelInfoLabel_;
            var existingPaceReference = existing.transform.parent.Find("PaceRef Scoreboard");
            if (existingPaceReference != null)
                NGUITools.Destroy(existingPaceReference.gameObject);

            var paceReference = NGUITools.AddChild(existing.transform.parent.gameObject);
            paceReference.name = "PaceRef Scoreboard";
            var label = paceReference.AddComponent<UILabel>();
            label.bitmapFont = existing.bitmapFont;
            label.trueTypeFont = existing.trueTypeFont;
            label.fontSize = existing.fontSize;
            label.color = existing.color;
            label.width = existing.width;
            label.pivot = existing.pivot;
            label.depth = existing.depth;
            label.overflowMethod = UILabel.Overflow.ResizeHeight;
            label.text = scoreboardData;
            label.MakePixelPerfect();

            const int gap = 8;

            var pos = existing.transform.localPosition;
            paceReference.transform.localPosition = new UnityEngine.Vector3(pos.x, pos.y - existing.height - gap, pos.z);

            var extraHeight = gap + label.height + gap;
            GrowPanelWidget(existing.transform.parent, extraHeight);

            ShiftLogoDown(existing.transform.parent, "UITexture - Medal", extraHeight / 2);
            ShiftLogoDown(existing.transform.parent, "Medal Logo", extraHeight / 2);
        }

        private static void ShiftLogoDown(UnityEngine.Transform parent, string childName, int shift)
        {
            var t = parent.Find(childName);
            if (t == null) return;
            var p = t.localPosition;
            t.localPosition = new UnityEngine.Vector3(p.x, p.y - shift, p.z);
        }

        private static void GrowPanelWidget(UnityEngine.Transform panelLevelInfo, int extraHeight)
        {
            var widget = panelLevelInfo.GetComponent<UIWidget>();
            if (widget == null) 
                return;
            
            if (widget.bottomAnchor.target != null)
                widget.bottomAnchor.absolute -= extraHeight;
        }
    }
}
