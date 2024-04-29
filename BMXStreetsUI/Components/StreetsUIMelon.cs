using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(BmxStreetsUI.Components.StreetsUIMelon), "BMX Streets UI", "version", "Author Name")]
[assembly: MelonGame()]

namespace BmxStreetsUI.Components
{
    public static class BuildInfo
    {
        public const string Name = "BmxStreetsUI"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Middleware for mods to integrate with the BMXStreets UI System"; // Description for the Mod.  (Set as null if none)
        public const string Author = "FrostyP/LineRyder"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class StreetsUIMelon : MelonMod
    {
        bool init;
        public override void OnInitializeMelon()
        {
            Log.logger = LoggerInstance;
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if(GameObject.Find(Constants.SystemTabSettings) != null && !init)
            {
                Log.Msg("StreetsUI Initializing");
                init = API.Initialize();
            }
        }
    }

}
