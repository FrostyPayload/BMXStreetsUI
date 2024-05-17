﻿using BmxStreetsUI.Components;
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
        
        
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!StreetsUI.IsReady)
            {
                if (sceneName.ToLower().Contains(Constants.MainMenuSceneName))
                {
                    if (StreetsUI.VersionNotAcceptable())
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
