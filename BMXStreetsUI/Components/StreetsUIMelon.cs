using BmxStreetsUI.Components;
using MelonLoader;

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
                    StreetsUI.Initialize();
                }
            }
        }

    }

}
