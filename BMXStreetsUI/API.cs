using BmxStreetsUI.Components;
using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using MelonLoader;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class API
    {
        static List<CustomMenu> CallsWaiting = new List<CustomMenu>();
        static GameObject? settingsPanel, settingsTab,MainMenu,ModMenu;
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
                ModMenuSetup();
                CompleteAwaiting();
            }
            else
            {
                Log.Msg("AwaitUI ending without completion");
            }
        }

        static void ModMenuSetup()
        {
            foreach (var menu in GameObject.FindObjectsOfType<MGMenu>(true).Where(menu => menu.gameObject.name == "Main Menu System"))
            {
                MainMenu = menu.gameObject;
            }

            var modmenu = BuildNewMenuBar();
            var ModMenuTab = new UICreator().CreateTab("Mods", settingsTab);
            if(modmenu == null || ModMenuTab == null)
            {
                Log.Msg($"Error setting up ModMenu", true);
                return;
            }
            modmenu.name = "StreetsUIModsMenu";
            var mg = modmenu.GetComponent<MGMenu>();
            LinkTabClickToAction(ModMenuTab, new Action(() => { modmenu.SetActive(true); mg.OpenMenu(); }));
            API.ModMenu = modmenu;
        }

        /// <summary>
        /// Build a new replica main menu top bar. Use a Tabcreator to make a new tab and use API.LinkTabClickToAction() to hook them up.
        /// then, use CreateMenuPanel() and CreateTab() to populate the bar
        /// </summary>
        /// <returns></returns>
        public static GameObject? BuildNewMenuBar()
        {
            if (MainMenu)
            {
                var ModMenu = GameObject.Instantiate(MainMenu);
                ModMenu.SetActive(false);
                ModMenu.transform.SetParent(MainMenu.transform.parent, false); // where all menus exist
                var mg = ModMenu.GetComponent<MGMenu>();
                mg.OnEnterOpen.RemoveAllListeners();
                mg.OnEnterClose.RemoveAllListeners();
                mg.OnOpenRaiseEvents.Clear();
                mg.OnCloseRaiseEvents.Clear();
                mg.selectCurrentOnEnable = true;
                mg.selectCurrentOnOpenMenu = true;
                // destroy existing tabs
                var toDestroyTabs = new Queue<GameObject>();
                foreach(var child in ModMenu.GetComponentsInChildren<EventTrigger>(true))
                {
                    toDestroyTabs.Enqueue(child.gameObject);
                    
                }
                while(toDestroyTabs.Count>0)
                {
                    UnityEngine.Object.DestroyImmediate(toDestroyTabs.Dequeue());
                }

                // Build return action (Button east, escape)
                var input = ModMenu.AddComponent<InputSystemEventCallback>();
                var settingsInput = settingsPanel.GetComponent<InputSystemEventCallback>();
                input._inputActionAsset = settingsInput._inputActionAsset;
                input.inputLayer = "UI";
                input._inputAction = null;
                input._inputActionName = "Cancel";
                input.actionName = "UI/Cancel";
                input._inputMap = "UI";
                input._inputLayerData = settingsInput._inputLayerData;
                input.OnActionPerformed = new UnityEngine.Events.UnityEvent();
                input.registerActionOnStart = false;
                input.registerOnEnableDisable = true;
                input.OnActionHeldPercentThrough_Float = new InputEventFloatCallback();
                input.OnActionHeld_Float = new InputEventFloatCallback();
                input.OnActionPerfomed_Int = new InputEventIntCallback();
                input.OnActionPerformedNegative = new UnityEngine.Events.UnityEvent();
                input.OnActionPerformedPositive = new UnityEngine.Events.UnityEvent();
                
                input.OnActionPerformed.AddListener(new System.Action(() => { ModMenu.SetActive(true); mg.OpenLastMenu();  }));
                input.RegisterAction();
                
                mg.Awake();
                mg.GetMenuSystem().Init();
                
                return ModMenu;
            }
            return null;
        }

        static void CompleteAwaiting()
        {
            Log.Msg($"Completing Awaiting UI Setups");
            foreach (CustomMenu menu in CallsWaiting)
            {
                CreatePanel(menu);
                if(menu.AutoLoad)
                {
                    menu.Load();
                }
            }
            CallsWaiting.Clear();
            CallsWaiting.TrimExcess();
        }

        /// <summary>
        /// Create a CustomMenu and use the other classes in this namespace to setup your menu, then bring it here to have it created and setup as a main menu option
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns>Did the creation succeed. On fail, check unity logs for info</returns>
        public static bool CreatePanel(CustomMenu newMenu, UICreator? creator = null,AutoTabSetup tabSetup = AutoTabSetup.ToModMenu)
        {
            if (init)
            {
                var panelCreator = creator;
                if(panelCreator == null)
                {
                    panelCreator = new UICreator();
                }
                var panel = panelCreator.CreatePanel(newMenu,settingsPanel);
                if (tabSetup == AutoTabSetup.ToMainMenu)
                {
                    var tab = panelCreator.CreateTab(newMenu.TabTitle, settingsTab);
                    LinkTabClickToAction(tab, newMenu.panel.OnOpen);
                }
                else if(tabSetup == AutoTabSetup.ToModMenu)
                {
                    var tab = panelCreator.CreateTab(newMenu.TabTitle, settingsTab,ModMenu.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
                    LinkTabClickToAction(tab, newMenu.panel.OnOpen);
                }
                return true;
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

        public static void LinkTabClickToAction(GameObject tab, Action action)
        {
            EventTrigger trigger = tab.GetComponent<EventTrigger>();
            foreach (var trig in trigger.triggers)
            {
                if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                {
                    if (trig.callback.m_PersistentCalls != null && trig.callback.m_PersistentCalls.Count > 0)
                    {
                        trig.callback.m_PersistentCalls.RemoveListener(0);
                    }
                    trig.callback.RemoveAllListeners();
                    trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                }
            }
            foreach (var trig in trigger.delegates)
            {
                if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                {
                    if (trig.callback.m_PersistentCalls != null && trig.callback.m_PersistentCalls.Count > 0)
                    {
                        trig.callback.m_PersistentCalls.RemoveListener(0);
                    }
                    trig.callback.RemoveAllListeners();
                    trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                }
            }

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

        public enum AutoTabSetup
        {
            ToMainMenu,ToModMenu,Custom
        }

    }

   

}
