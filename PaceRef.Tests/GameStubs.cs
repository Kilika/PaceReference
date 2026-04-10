// Minimal stubs so ScoreboardBuilder.cs compiles without game DLLs.
// Only the Build() method is under test; IsTimeMode/GetFormatter are
// tested indirectly via the helper methods on ScoreboardBuilderTests.

namespace PaceRef
{
    /// <summary>Stub for the game's GameModeID enum.</summary>
    internal enum GameModeID
    {
        None,
        Sprint,
        Stunt,
        Soccer,
        FreeRoam,
        ReverseTag,
        LevelEditorPlay,
        CoopSprint,
        Challenge,
        Adventure,
        SpeedAndStyle,
        Trackmogrify,
        Demo,
        MainMenu,
        LostToEchoes,
        Nexus,
        TheOtherSide,
        Count,
    }

    /// <summary>Stub for the game's GUtils class.</summary>
    internal static class GUtils
    {
        /// <summary>
        /// Formats raw milliseconds as mm:ss.fff — mirrors the game's formatting
        /// closely enough for test assertions without requiring Unity.
        /// </summary>
        public static string GetFormattedMS(double ms)
        {
            if (ms < 0) ms = 0;
            int totalMs = (int)ms;
            int minutes = totalMs / 60000;
            int seconds = (totalMs % 60000) / 1000;
            int millis  = totalMs % 1000;
            return $"{minutes}:{seconds:D2}.{millis:D3}";
        }
    }
}
