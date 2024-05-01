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
        static List<(CustomMenu,AutoTabSetup)> CallsWaiting = new List<(CustomMenu, AutoTabSetup)>();
        static GameObject? settingsPanel, settingsTab,MainMenu,ModMenu;
        public static bool init { get; private set; }

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
            foreach ((CustomMenu menu,AutoTabSetup setup) in CallsWaiting)
            {
                CreatePanel(menu,setup);
                if(menu.AutoLoad)
                {
                    menu.Load();
                }
            }
            CallsWaiting.Clear();
            CallsWaiting.TrimExcess();
        }

        /// <summary>
        /// Simple Setup, Pass your Menu in here to have it auto setup to the Mod tab, optionally you can choose to auto setup to the main menu or choose custom to just get the panel without tab
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns>Returns true in two instances, 1) the panel was created and is ready to use. 2) The UI isn't ready yet but the API is holding your menu and will create it when the UI is loaded.
        /// In this case you will need to wait until your menu is populated before using it</returns>
        public static bool CreatePanel(CustomMenu newMenu,AutoTabSetup tabSetup = AutoTabSetup.ToModMenu, UICreator? creator = null)
        {
            if (init)
            {
                var UICreator = creator;
                if(UICreator == null)
                {
                    UICreator = new UICreator();
                }
                var panel = UICreator.CreatePanel(newMenu,settingsPanel);
                if (tabSetup == AutoTabSetup.ToMainMenu)
                {
                    var tab = UICreator.CreateTab(newMenu.TabTitle, settingsTab);
                    LinkTabClickToAction(tab, newMenu.panel.OnOpen);
                }
                else if(tabSetup == AutoTabSetup.ToModMenu)
                {
                    var tab = UICreator.CreateTab(newMenu.TabTitle, settingsTab,ModMenu.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
                    LinkTabClickToAction(tab, newMenu.panel.OnOpen);
                }
                newMenu.OnMenuCreation?.Invoke(newMenu);
                return true;
            }
            else
            {
                if (!CallsWaiting.Contains((newMenu,tabSetup)))
                {
                    CallsWaiting.Add((newMenu, tabSetup));
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sets up the event trigger on your tab to fire the open function of your menu
        /// </summary>
        /// <param name="triggerObj"></param>
        /// <param name="action"></param>
        public static void LinkTabClickToAction(GameObject triggerObj, Action action)
        {
            EventTrigger trigger = triggerObj.GetComponent<EventTrigger>();
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
