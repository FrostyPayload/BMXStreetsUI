using BmxStreetsUI.Components;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(StreetsUIMelon), "BmxStreetsUI", "1.0.0", "FrostyP/LineRyder")]
[assembly: MelonGame()]

namespace BmxStreetsUI.Components
{
    internal static class BuildInfo
    {
        public const string Name = "BmxStreetsUI"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Middleware for mods to integrate with the BMXStreets UI System"; // Description for the Mod.  (Set as null if none)
        public const string Author = "FrostyP/LineRyder"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    internal class StreetsUIMelon : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Log.logger = LoggerInstance;

        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!StreetsUI.IsReady)
            {
                if (sceneName.ToLower().Contains(Constants.MainMenuSceneName))
                {
                    StreetsUI.Initialize();
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
        }
    }

}
