using BmxStreetsUI.Components;
using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class API
    {
        public static GameObject? settingsPanel, settingsTab,MainMenu;
        public static Queue<Action> OnCreation = new Queue<Action>();
        public static bool IsReady { get { return settingsTab != null && settingsPanel != null && MainMenu != null; } }

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
            }

            Components.ModMenu.ModMenuSetup(settingsTab);
            ModMenu.CharacterMenuSetup();
            CompleteAwaiting();
            Log.Msg("AwaitUI Completed");
            
        }

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
                
                mg.Awake();
                mg.GetMenuSystem().Init();
                
                return ModMenu;
            }
            return null;
        }
        static void CompleteAwaiting()
        {
            Log.Msg($"Running OnCreation callbacks");
            while(OnCreation.Count>0)
            {
                try
                {
                    OnCreation.Dequeue().Invoke();
                }
                catch(Exception ex)
                {
                    Log.Msg($"Setup error on creation: {ex}",true);
                }
            }  
            OnCreation.TrimExcess();
        }
        /// <summary>
        /// If API.IsReady is false when your mod is checking (before the UI exists) you can register a callback here to start your mods code
        /// </summary>
        /// <param name="callback"></param>
        public static void RegisterForUICreation(Action callback)
        {
            OnCreation.Enqueue(callback);
        }

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
            if (group.SelectCallBack != null) smartList.OnSelected.AddListener(group.SelectCallBack);

            return smartList;
        }

        public static GameObject? CreatePanel(MenuPanel newMenu,AutoTabSetup tabSetup = AutoTabSetup.ToModMenu,bool SetupSaveOnMainMenuExit = true,bool LoadOnCreate = true)
        {
            if (IsReady)
            {
                Log.Msg($"Setting up new StreetsUI");
                var Panel = UnityEngine.Object.Instantiate(settingsPanel);
                if (Panel == null)
                {
                    return null;
                }
                var UIPanel = Panel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                UIPanel.Init();
                var listSet = MenuToSmartSet(newMenu);
                UIPanel.PanelName = newMenu.TabTitle;
                UIPanel.listSet = listSet;
                newMenu.panel = UIPanel;
                UIPanel.transform.SetParent(settingsPanel.transform.parent, false);

                if (newMenu.overridePallete) UIPanel.SetPallete(newMenu.palette);

               
                if (tabSetup == AutoTabSetup.ToMainMenu)
                {
                    var tab = CreateTab(newMenu.TabTitle);
                    LinkTabClickToAction(tab, newMenu.panel.OnOpen);
                }
                else if(tabSetup == AutoTabSetup.ToModMenu)
                {
                    Components.ModMenu.AddToModMenu(newMenu);
                }
                else if(tabSetup == AutoTabSetup.ToCharacter)
                {
                    ModMenu.AddToCharacterMenu(newMenu);
                }
                if (tabSetup != AutoTabSetup.Custom) UIPanel.RunSetup();
                if(newMenu.OnMenuOpen != null) newMenu.SetPanelOnOpenCallback(newMenu.OnMenuOpen);
                if (newMenu.OnMenuClose != null) newMenu.SetPanelOnCloseCallback(newMenu.OnMenuClose);
                if (newMenu.OnTabChange != null) newMenu.SetPanelOnTabChangedCallback(newMenu.OnTabChange);
                if (newMenu.OnSelectionChange != null) newMenu.SetPanelOnSelectionChangedCallback(newMenu.OnSelectionChange);
                if (SetupSaveOnMainMenuExit) RegisterToMainMenuClose(new System.Action(() => { newMenu.Save(); }));
                if (LoadOnCreate) newMenu.Load();
                return Panel;
            }
            else
            {
                ///
                return null;
            }
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

       /// <summary>
       /// Hook into the Submit events of the tab
       /// </summary>
       /// <param name="triggerObj"></param>
       /// <param name="action"></param>
       /// <param name="deleteOthers"></param>
        public static void LinkTabClickToAction(GameObject triggerObj, Action action, bool deleteOthers = true)
        {
            EventTrigger trigger = triggerObj.GetComponent<EventTrigger>();
            foreach (var trig in trigger.triggers)
            {
                if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                {
                    if (deleteOthers)
                    {
                        if (trig.callback.m_PersistentCalls != null && trig.callback.m_PersistentCalls.Count > 0)
                        {
                            trig.callback.m_PersistentCalls.RemoveListener(0);
                        }
                        trig.callback.RemoveAllListeners();
                    }
                    trig.callback.RemoveListener(new System.Action<BaseEventData>(data => action.Invoke()));
                    trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                }
            }
            foreach (var trig in trigger.delegates)
            {
                if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                {
                    if (deleteOthers)
                    { 
                        if (trig.callback.m_PersistentCalls != null && trig.callback.m_PersistentCalls.Count > 0)
                        {
                            trig.callback.m_PersistentCalls.RemoveListener(0);
                        }
                        trig.callback.RemoveAllListeners();
                    }
                    trig.callback.RemoveListener(new System.Action<BaseEventData>(data => action.Invoke()));
                    trig.callback.AddListener(new System.Action<BaseEventData>(data => action.Invoke()));
                }
            }

        }
        public static void LinkTabClickToMenuOpen(GameObject tab, GameObject menu)
        {
            if(menu.GetComponent<MGMenu>() != null)
            {
                LinkTabClickToAction(tab, new System.Action(() => { menu.GetComponent<MGMenu>().OpenMenu(); }));
            }
        }

        public enum AutoTabSetup
        {
            ToMainMenu,ToModMenu,ToCharacter,Custom
        }
        public enum SystemTab
        {
            General,Audio,Video,Gameplay
        }
    }

}
