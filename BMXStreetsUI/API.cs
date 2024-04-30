using BmxStreetsUI.Components;
using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class API
    {
        static List<CustomMenu> CallsWaiting = new List<CustomMenu>();
        static GameObject? settingsPanel, settingsTab;
        internal static bool init { get; private set; }
        internal static void Initialize()
        {
            Log.Msg($"API Initialize");
            MelonCoroutines.Start(AwaitUIMenus());
            
        }
        static System.Collections.IEnumerator AwaitUIMenus()
        {
            while(settingsPanel == null || settingsTab == null)
            {
                yield return new WaitForSeconds(0.5f);
                Log.Msg("Awaiting MainMenu Scene objects");
                foreach(var menu in GameObject.FindObjectsOfType<MGMenu>(true).Where(obj => obj.gameObject.name.ToLower().Contains(Constants.SystemTabSettings)))
                {
                    settingsPanel = menu.gameObject;
                    break;
                }
                foreach (var trigger in GameObject.FindObjectsOfType<EventTrigger>(true).Where(obj => obj.gameObject.name.ToLower().Contains(Constants.SystemTab)))
                {
                    settingsTab = trigger.gameObject;
                    break;
                }
            }

            if(settingsPanel != null && settingsTab != null)
            {
                init = true;
                CompleteAwaiting();
            }
            else
            {
                Log.Msg("AwaitUI ending without completion");
            }
        }
        static void CompleteAwaiting()
        {
            Log.Msg($"Completing Awaiting UI Setups");
            foreach (CustomMenu menu in CallsWaiting)
            {
                Create(menu, new PanelCreator(settingsTab,settingsPanel));
            }
            CallsWaiting.Clear();
            CallsWaiting.TrimExcess();
        }
        /// <summary>
        /// Create a CustomMenu and use the other classes in this namespace to setup your menu, then bring it here to have it created and setup
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns>Did the creation succeed. On fail, check unity logs for info</returns>
        public static bool AddMenu(CustomMenu newMenu)
        {
            if (init)
            {
                return Create(newMenu, new PanelCreator(settingsTab,settingsPanel));
            }
            else
            {
                if (!CallsWaiting.Contains(newMenu))
                {
                    CallsWaiting.Add(newMenu);
                    return true;
                }
                return false;
            }
        }
       
        static bool Create(CustomMenu menu, PanelCreator creator)
        {
            return creator.Create(menu);
        }
        /// <summary>
        /// Retreive a menu using the name set in CustomMenu.panelTitle
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns>The menu if found, or null otherwise</returns>
        public static CustomMenu? GetMenu(string menuName)
        {
            return null;
        }
        public static bool RemoveMenu(CustomMenu menu)
        {
            return false;
        }

    }

   

}
