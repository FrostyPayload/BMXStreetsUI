using Il2Cpp;
using Il2CppMG_Core.MG_SmartData.SaveLoad;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI
{
    [RegisterTypeInIl2Cpp]
    public class UIPanel : MonoBehaviour
    {
        public UIPanel(IntPtr ptr) : base(ptr) { }
        public UIPanel() : base(ClassInjector.DerivedConstructorPointer<UIPanel>()) => ClassInjector.DerivedConstructorBody(this);

        public GameObject characterPrefab, character, ghost, ghostPrefab, bmx, listroot, mainCanvas, tabParent;

        public GameObject tab;
        DataConfigPanel config;
        /// <summary>
        /// An object that sticks around through duplication, contains prefabs of the slider,button etc.
        /// Retreived through DataConfigPanel present on the panel object
        /// </summary>
        protected SmartUIPaletteData pallete;
        /// <summary>
        /// References a list of SmartDataContainerReferenceList, Each list turns into the options on a tab
        /// </summary>
        protected SmartDataContainerReferenceListSet listSet;
        /// <summary>
        /// talks to the system to turn on and off etc, registers itself by looking for parent
        /// </summary>
        protected MGMenu menu;
        /// <summary>
        /// The duplicate objects have open and close triggers mapped to the original objects
        /// </summary>
        protected Action<BaseEventData> openMenuTrigger, CloseMenu;
        protected Action closeMenuTrigger;

        Action<object> dataChangeCallbackData; // the callback we want updates through when using UI

        protected string TabName = "FrostyUITab";
        protected string PanelName = "FrostyUI panel";

        public void Awake()
        {
            Debug.Log("StreetsUI Panel awake");


            var content = transform.FindDeepChild("Content");
            foreach (var trans in content.GetComponentsInChildren<Transform>(true))
            {
                if (trans.name.ToLower().Contains("data panel"))
                {
                    //Destroy(trans.gameObject);
                }
            }

            if (tab == null)
            {
                Debug.Log("Tab is null in Grinds Setup");
                return;
            }
            if (!GetReferences()) { Debug.Log("References failed"); return; }
            SetupTriggers();
            if (!SetupTab()) { return; }
            SetupData();
            SetupConfigPanel("Grind Poses");
            SetupPanel();
            SetDataCallbacks();
            var saveLoad = Singleton<SaveLoadManager>.GetInstance();
            if (saveLoad != null)
            {
                Debug.Log($"Save load system found, adding data");
                saveLoad.dataList.Add(listSet);
            }

        }
        void SetupTriggers()
        {
            EventTrigger trigger = tab.GetComponent<EventTrigger>();
            openMenuTrigger = delegate { OnOpen(); };
            closeMenuTrigger = delegate { OnClose(); };
            CloseMenu = delegate { OnClose(); };
            foreach (var trig in trigger.triggers)
            {
                if (trig.eventID == EventTriggerType.Submit | trig.eventID == EventTriggerType.PointerClick)
                {
                    if (trig.callback.m_PersistentCalls != null && trig.callback.m_PersistentCalls.Count > 0)
                    {
                        trig.callback.m_PersistentCalls.RemoveListener(0);
                    }
                    trig.callback.RemoveAllListeners();
                    trig.callback.AddListener(openMenuTrigger);
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
                    trig.callback.AddListener(openMenuTrigger);
                }
            }

        }

        void OnOpen()
        {
            Debug.Log("FrostyUI panel opening");
            if (menu.GetMenuSystem() == null)
            {
                Debug.LogError($"No menu System found in frostyUI OnOpen, GameObject {gameObject.name}");
                return;
            }
            menu.OpenMenu();
            InputSystemEventCallback inputCancel = GetComponent<InputSystemEventCallback>();
            inputCancel.OnActionPerformed.RemoveAllListeners();
            inputCancel.OnActionPerformed.AddListener(closeMenuTrigger);

        }
        void OnClose()
        {
            Debug.Log("FrostyUI Panel closing");
            if (menu.linkedPreviousMenu == null)
            {
                Debug.Log($"Last menu is null");
            }
            menu.OpenLastMenu();
            InputSystemEventCallback inputCancel = GetComponent<InputSystemEventCallback>();
            inputCancel.OnActionPerformed.RemoveAllListeners();
            inputCancel.OnActionPerformedPositive.RemoveAllListeners();
            gameObject.SetActive(false);
        }
        bool GetReferences()
        {
            mainCanvas = GameObject.Find("Main Menu Canvas");
            config = GetComponent<DataConfigPanel>();
            menu = GetComponent<MGMenu>();
            if (mainCanvas == null) { Debug.Log($"Main canvas is not found"); return false; }
            if (tab == null) { Debug.Log($"Tab is not found in getReferences"); return false; }
            var smartui = tab.GetComponent<SmartUIBehaviour>();
            var manager = smartui._smartUIManagerData;
            pallete = manager._paletteData;
            listroot = config.listRoot.gameObject;
            smartui.UnRegisterEvents();
            return true;
        }
        void SetupPanel()
        {
            Debug.Log($"Setup frostyUIPanel {PanelName}");
            foreach (var transform in transform)
            {
                if (transform is Transform)
                {
                    var trans = (Transform)transform;
                    if (trans.gameObject.GetComponent<LocalizeStringEvent>() != null)
                    {
                        trans.gameObject.GetComponent<LocalizeStringEvent>().enabled = false;
                    }
                }
            }

            gameObject.name = PanelName;
            transform.SetParent(mainCanvas.transform, false);
            if (menu == null) { Debug.Log($"No MGmenu"); return; }
            //menu.Awake();
            //menu.OnEnterOpen.RemoveAllListeners();
            //if(menu.OnOpenRaiseEvents != null) menu.OnOpenRaiseEvents.Clear();
            //menu.Init();

            //UIHorizontalSelectorSmartSetBehaviour selector = GetComponentInChildren<UIHorizontalSelectorSmartSetBehaviour>(true);
            //selector._DataReferenceSets = listSet;
        }
        protected virtual void SetupData()
        {
           // do all smartdata setup
        }

        void SetupConfigPanel(string PanelLabel)
        {
            if (config != null)
            {
                Debug.Log("Setting up config panel");
                config.panelLabel.text = PanelLabel;
                config.configDatas = listSet;
                config.configDatas_DevAlt = null;
                config._hasDevAltData = false;
                config._populateOnEnable = true;
                config.useObjectList = false;
                config.currentConfigDatas = null;
                config.currentConfigData = null;
                config.currentDataIndex = 0;
                config.OnChangedSelection = new UnityEvent();
                config.OnChangeSelected_Int = new DataConfigPanelIntCallback();
                config.Start();
                config.Init();
                config.Validate();
            }
        }
        bool SetupTab()
        {
            if (tab == null) { Debug.Log("System tab not found"); return false; }
            // configure clones settings tab
            tab.name = "FrostyUIPanel";
            tab.GetComponent<LocalizeStringEvent>().enabled = false;
            tab.GetComponentInChildren<TextMeshProUGUI>().text = "Grinds";
            tab.transform.SetParent(tabParent.transform, false);
            return true;
        }
        void SetDataCallbacks()
        {
            if (!listSet) { Debug.Log($"StreetsUI: No Listset in smartData Callback : {gameObject.name}"); return; }
            dataChangeCallbackData = delegate (object obj) { Callback(obj); };
            foreach (var refList in listSet._DataRefLists)
            {
                foreach(var data in refList._dataContainer._smartDatas)
                {
                    if (data.OnValueChanged_DataValue != null)
                    {
                        // can we remove all?
                        data.OnValueChanged_DataValue.AddListener(dataChangeCallbackData);
                    }
                }

            }
        }
        /// <summary>
        /// Should be receiving on this when smartdata is changed, rogue event with null reference in the way?
        /// </summary>
        /// <param name="value"></param>
        public void Callback(object value)
        {
            if (value == null) { Debug.Log("Received null in callback"); return; }
            Debug.Log($"Receiving callback : object = {value.GetType().ToString()}");
            if (value is SmartData<SmartDataFloatStuct>) Debug.Log("Val is smart<floatstruct>");
            if (value is SmartData) Debug.Log("Val is smartData");
        }

    }
    public enum GrindType
    {
        smith, feeble, pegs, crook, crank, luce, unluce, magiccarpet, ice, tooth
    }
}