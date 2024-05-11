using BmxStreetsUI.Components;
using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class StreetsUI
    {
        public static GameObject? settingsPanel, settingsTab,MainMenu,quickMenu;
        static List<Action> OnCreation = new List<Action>();
        public static bool IsReady { get { return settingsTab != null && settingsPanel != null && MainMenu != null && quickMenu != null; } }

        internal static void Initialize()
        {
            Log.Msg($"API Initialize");
            MelonCoroutines.Start(AwaitUIMenus());
            
        }
        static System.Collections.IEnumerator AwaitUIMenus()
        {
            while(!IsReady)
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
                foreach (var menu in GameObject.FindObjectsOfType<MGMenu>(true).Where(menu => menu.gameObject.name == "Main Menu System"))
                {
                    MainMenu = menu.gameObject;
                }
                foreach (var menu in GameObject.FindObjectsOfType<MGMenuSystem>(true).Where(menu => menu.gameObject.name == Constants.QuickMenu))
                {
                    quickMenu = menu.gameObject;
                }
            }

            Components.ModMenu.ModMenuSetup(settingsTab);
            ModMenu.CharacterMenuSetup();
            CompleteAwaiting();
            
            Log.Msg("Initialise Completed");
        }
        /// <summary>
        /// When the start button is pressed an the main menu opens, Requires API.IsReady to be true, use API.IsReady or RegisterForUICreation() to wait for this to be true.
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterToMainMenuOpen(Action callback)
        {
            if(MainMenu != null)
            {
                var sys = MainMenu.GetComponent<MGMenu>().GetMenuSystem();
                if(sys != null)
                {
                    sys.OnOpen.AddListener(callback);
                }
            }
        }
        /// <summary>
        /// When the start button is pressed an the main menu closes, Requires API.IsReady to be true, use API.IsReady or RegisterForUICreation() to wait for this to be true.
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterToMainMenuClose(Action callback)
        {
            if (MainMenu != null)
            {
                var sys = MainMenu.GetComponent<MGMenu>().GetMenuSystem();
                if (sys != null)
                {
                    sys.OnClose.AddListener(callback);
                }
            }
        }
        public static bool InjectOptionToSystemPanel(MenuOptionBase option, SystemTab tab)
        {
            if (!IsReady)
            {
                ////
                return false;
            }
            foreach(var container in Resources.FindObjectsOfTypeAll<SmartDataContainer>())
            {

            }
            
            return true;
        }
        /// <summary>
        /// Build a new replica main menu top bar.
        /// </summary>
        /// <returns></returns>
        public static GameObject? CreateMenuBar()
        {
            if (IsReady && MainMenu != null)
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
                mg.currentMenuSelectable = null;
                mg.isFirstMenu = false;
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
                
                input.OnActionPerformed.AddListener(new System.Action(() => {  mg.OpenLastMenu();  }));
                input.RegisterAction();

                mg.OnDisable();
                mg.Awake();
                mg.GetMenuSystem().Init();
                
                return ModMenu;
            }
            return null;
        }
        static void CompleteAwaiting()
        {
            Log.Msg($"Running OnCreation callbacks");
            foreach(var uijob in OnCreation)
            {
                try
                {
                    uijob.Invoke();
                }
                catch(Exception ex)
                {
                    Log.Msg($"Setup error on creation: {ex}",true);
                }
            }  
            if(EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(settingsTab);
            }
        }
        /// <summary>
        /// If API.IsReady is false when your mod is checking (before the UI exists) you can register a callback here to start your mods code
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterForUICreation(Action callback)
        {
            OnCreation.Add(callback);
        }
        /// <summary>
        /// The returned object is the full package of data used by the UIPanel to create it's menu when opened with events setup. Mash's DataConfigPanel is the ultimate endpoint it needs to reach.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public static SmartDataContainerReferenceListSet MenuToSmartSet(MenuPanel menu)
        {
            var listSet = SmartDataManager.CreateNewSet(menu.TabTitle);
            foreach (var group in menu.Groups)
            {
                var smartList = GroupToSmartList(group, menu.TabTitle);
                listSet._DataRefLists.Add(smartList);
            }
            return listSet;

        }

        /// <summary>
        /// The returned SmartData is Serializable and either a SmartData_Float or SmartData_Button if you pass in a button.
        /// The Id is used for inGame saveLoad and needs to be unique within a container.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="uniqueToContainerID"></param>
        /// <returns></returns>
        public static SmartData OptionToSmartUI(MenuOptionBase option, string uniqueToContainerID)
        {
            switch (option.UIStyle)
            {
                case UIStyle.Slider:
                    return SmartDataManager.SetupSmartUI(SmartDataManager.CreateDefaultSmartFloat(option.title), uniqueToContainerID, option, SmartDataFloatStuct.DataStyle.Free, SmartData.DataUIStyle.Slider);
                case UIStyle.SteppedInt:
                    return SmartDataManager.SetupSmartUI(SmartDataManager.CreateDefaultSmartFloat(option.title), uniqueToContainerID, option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);
                case UIStyle.Button:
                    return SmartDataManager.SetupButton(SmartDataManager.CreateDefaultSmartData<SmartData_Button>(option.title), option);
                case UIStyle.Toggle:
                    return SmartDataManager.SetupSmartUI(SmartDataManager.CreateDefaultSmartFloat(option.title), uniqueToContainerID, option, SmartDataFloatStuct.DataStyle.Stepped, SmartData.DataUIStyle.Stepped);

            }
            return ScriptableObject.CreateInstance<SmartData_Float>();
        }

        /// <summary>
        /// The returned object is directly SaveLoadable. The list inside contains all your options as SmartData, each of them holds the events that will fire on change of their value.
        /// Calling Load immediatley will attempt to populate your options with matching data from disk using the folderName and group.Title.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static SmartDataContainerReferenceList GroupToSmartList(OptionGroup group, string folderName)
        {
            var container = SmartDataManager.CreateNewContainer(group.title, folderName);
            var smartList = SmartDataManager.CreateNewList(group.title);
            foreach (var option in group.options)
            {
                var data = OptionToSmartUI(option, $"{group.title}_{option.title}");
                container._smartDatas.Add(data);
            }
            container.ValidateList();
            smartList.connectedContainer = container;
            smartList._dataContainer = container;
            smartList.dataGroup = container._smartDatas;
            smartList.OnSelected = new UnityEvent();
            
            return smartList;
        }

        public static GameObject? CreatePanel(MenuPanel newMenu, AutoTabSetup tabSetup = AutoTabSetup.ToModMenu, bool SetupSaveOnMainMenuExit = true, bool LoadOnCreate = true, MenuPanel? shareDataWith = null)
        {
            if (!IsReady) return null;

            Log.Msg($"Setting up new StreetsUI");
            var Panel = UnityEngine.Object.Instantiate(settingsPanel);
            if (Panel == null)
            {
                return null;
            }
            var uipanel = Panel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
            newMenu.panel = uipanel;
            var parentTo = tabSetup == AutoTabSetup.ToQuickAccess ? quickMenu.transform.FindDeepChild(Constants.QuickAccessMenuParent) : settingsPanel.transform.parent;
            uipanel.transform.SetParent(parentTo, false);
            uipanel.Init();
            uipanel.listSet = shareDataWith == null ? MenuToSmartSet(newMenu) : shareDataWith.panel.listSet;
            uipanel.PanelName = newMenu.TabTitle;

            if (newMenu.overridePallete) uipanel.SetPallete(newMenu.palette);

            if (tabSetup == AutoTabSetup.ToMainMenu)
            {
                var tab = CreateTab(newMenu.TabTitle);
                LinkTabTriggerToAction(tab, newMenu.panel.OnOpen);
            }
            else if (tabSetup == AutoTabSetup.ToModMenu)
            {
                Components.ModMenu.AddToModMenu(newMenu);
            }
            else if (tabSetup == AutoTabSetup.ToCharacter)
            {
                ModMenu.AddToCharacterMenu(newMenu);
            }
            if (tabSetup != AutoTabSetup.Custom) uipanel.RunSetup();
            uipanel.OnOpenEvent.AddListener(new System.Action<int>((val) => { newMenu.MenuOpenedEvent(val); }));
            uipanel.OnCloseEvent.AddListener(new System.Action<int>((val) => { newMenu.MenuClosedEvent(val); }));
            uipanel.OnTabChangedEvent.AddListener(new System.Action<int>((val) => { newMenu.TabChangedEvent(val); }));
            if (SetupSaveOnMainMenuExit) RegisterToMainMenuClose(new System.Action(() => { Log.Msg($"{newMenu.TabTitle} Saving");  newMenu.Save(); }));
            if (LoadOnCreate) newMenu.Load();
            return Panel;
        }

        /// <summary>
        /// Parents to the mod menu if parent is null
        /// </summary>
        /// <param name="TabLabel"></param>
        /// <param name="parent"></param>
        /// <returns>The setup tab which has an EventTrigger component, use LinkTabClickToAction() to link the trigger</returns>
        public static GameObject? CreateTab(string TabLabel, Transform parent = null)
        {
            if (!IsReady) return null;
            var toParent = parent == null ? ModMenu.GetModMenuTabParent() : parent;
            var tab = UnityEngine.Object.Instantiate(settingsTab);
            tab.transform.SetParent(toParent, false);
            tab.GetComponent<LocalizeStringEvent>().enabled = false;
            tab.GetComponentInChildren<TextMeshProUGUI>().text = TabLabel;
            tab.name = TabLabel + "Object";
            var smartui = tab.GetComponent<SmartUIBehaviour>();
            smartui.UnRegisterEvents();

            return tab;
        }

        public static GameObject? CreateQuickMenuButton(string name = "",string spriteName = "Lightbulb")
        {
            var settingsbutton = quickMenu.transform.FindDeepChild("Settings").gameObject;
            if (settingsbutton != null)
            {
                var newButton = GameObject.Instantiate(settingsbutton);
                newButton.name = name == "" ? "StreetsUIQuickMenuButton" : name;
                var img = newButton.GetComponentInChildren<Image>();
                var btn = newButton.GetComponent<UnityEngine.UI.Button>();
                btn.onClick.RemoveAllListeners();
                btn.m_OnClick.RemoveAllListeners();
                GameObject.DestroyImmediate(newButton.GetComponentInChildren<UnityGameEventListener>().gameObject);
                var sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where((sprite)=> { return sprite.name.Contains(spriteName); });
                if(sprite != null)
                {
                    img.sprite = sprite.FirstOrDefault();
                }
                newButton.transform.SetParent(settingsbutton.transform.parent, false);
                // decouple
                LinkTabTriggerToAction(newButton, new System.Action(() => { Log.Msg($"UnLinked button clicked"); }));
                return newButton;
            }
            return null;
        }

       /// <summary>
       /// Hook into the Submit events of the tab
       /// </summary>
       /// <param name="triggerObj"></param>
       /// <param name="action"></param>
       /// <param name="deleteOthers"></param>
        public static void LinkTabTriggerToAction(GameObject triggerObj, Action action, bool deleteOthers = true)
        {
            EventTrigger trigger = triggerObj.GetComponent<EventTrigger>();
            if(!deleteOthers )
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Submit;
                entry.callback.AddListener(new System.Action<BaseEventData>((data) => { action.Invoke(); }));
                //trigger.triggers.Add(entry);
                trigger.delegates.Add(entry);
            }
            else
            {
                foreach (var trig in trigger.triggers)
                {
                    if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                    {
                        if (deleteOthers)
                        {
                            trig.callback.RemoveAllListeners();
                        }
                        //trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                    }
                }
                foreach (var trig in trigger.delegates)
                {
                    if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                    {
                        if (deleteOthers)
                        { 
                            trig.callback.RemoveAllListeners();
                        }
                        trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                    }
                }
            }
            trigger.MarkDirty();
            trigger.Finalize();
        }
        public static void LinkTabTriggerToUIPanel(GameObject tab, GameObject uiPanel)
        {
            if(uiPanel.GetComponent<MGMenu>() != null)
            {
                LinkTabTriggerToAction(tab, new System.Action(() => { uiPanel.SetActive(true); uiPanel.GetComponent<UIPanel>().OnOpen(); }));
            }
        }
        public static void LinkQuickButtonToAction(GameObject button, Action action)
        {
            var uibutton = button.GetComponent<UnityEngine.UI.Button>();
            if (uibutton == null)
            {
                return;
            }
            uibutton.onClick.RemoveAllListeners();
            uibutton.m_OnClick.RemoveAllListeners();
            uibutton.onClick.AddListener(action);

        }
        public static void LinkQuickButtonToUIPanel(GameObject button, GameObject uiPanel)
        {
            LinkQuickButtonToAction(button,new System.Action(() => { uiPanel.SetActive(true); uiPanel.GetComponent<UIPanel>().OnOpen(); }));
        }

        public enum AutoTabSetup
        {
            ToMainMenu,ToModMenu,ToCharacter,ToQuickAccess,Custom
        }
        public enum SystemTab
        {
            General,Audio,Video,Gameplay
        }
    }

}
