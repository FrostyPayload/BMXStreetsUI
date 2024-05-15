using BmxStreetsUI.Components;
using Il2CppTriangleNet;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(StreetsUIMelon), "BmxStreetsUI", "1.0.0", "FrostyP/LineRyder")]
[assembly: MelonGame()]

namespace BmxStreetsUI.Components
{
    internal class StreetsUIMelon : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Log.logger = LoggerInstance;
        }
        
        bool VersionNotAcceptable()
        {
            string minVer = "1.0.0.136.0";
            string versionString = Application.version;
            // Find the first occurrence of "[" and "]"
            int startIndex = versionString.IndexOf('[');
            int endIndex = versionString.IndexOf(']');

            // Extract the substring between "[" and "]"
            string versionNumber = versionString.Substring(startIndex + 1, endIndex - startIndex - 1);

            return versionNumber.CompareTo(minVer) < 0;
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!StreetsUI.IsReady)
            {
                if (sceneName.ToLower().Contains(Constants.MainMenuSceneName))
                {
                    if (VersionNotAcceptable())
                    {
                        LoggerInstance.Error($"BMXStreetsUI shut down due to internal error");
                        return;
                    }

                    StreetsUI.Initialize();
                }
            }
        }

    }

}
