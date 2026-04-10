using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace PaceRef
{
    [BepInPlugin(Constants.PluginGuid, Constants.PluginName, "1.0.0")]
    public sealed class Mod : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; }
        internal static ConfigEntry<int> SampleSizeConfig { get; set; }
        
        private void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(Constants.PluginName);
            ConfigDescription configDescription = new ConfigDescription(
                description: Constants.Config.SampleSizeDescription,
                acceptableValues: new AcceptableValueRange<int>(10, 1000)
            );

            SampleSizeConfig = Config.Bind(
                section: Constants.Config.SectionName,
                key: Constants.Config.SampleSize,
                defaultValue: 100, 
                configDescription: configDescription);
            
            Harmony harmony = new Harmony(Constants.HarmonyId);
            Logger.LogInfo("Loading PaceRef...");
            harmony.PatchAll();
            Logger.LogInfo("PaceRef loaded successfully!");
        }
    }
}