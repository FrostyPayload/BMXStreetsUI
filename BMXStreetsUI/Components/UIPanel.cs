using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using UnityEngine;
using UnityEngine.Events;
using Il2CppInterop.Runtime.Injection;
using UnityEngine.Localization.Components;
using Il2CppMichsky.UI.ModernUIPack;

namespace BmxStreetsUI.Components
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIPanel : MonoBehaviour
    {
        public UIPanel(IntPtr ptr) : base(ptr) { }
        public UIPanel() : base(ClassInjector.DerivedConstructorPointer<UIPanel>()) => ClassInjector.DerivedConstructorBody(this);

        public DataConfigPanel config;
        /// <summary>
        /// References a list of SmartDataContainerReferenceList, Each list turns into the options on a tab
        /// </summary>
        public SmartDataContainerReferenceListSet listSet;
        /// <summary>
        /// talks to the system to turn on and off etc, registers itself by looking for parent
        /// </summary>
        public MGMenu menu;
        public string PanelName = "StreetsUIpanel";

        public UnityEvent<int> OnOpenEvent, OnCloseEvent,OnTabChangedEvent,OnSelectionChangedEvent;

        public SmartDataContainerReferenceListSet extraListSet;
        public void Init()
        {
            OnOpenEvent = new UnityEvent<int>();
            OnCloseEvent = new UnityEvent<int>();
            OnTabChangedEvent = new UnityEvent<int>();
            OnSelectionChangedEvent = new UnityEvent<int>();
            extraListSet = SmartDataManager.CreateNewSet($"{PanelName}_ExtraSet_ListSet");
            DontDestroyOnLoad(extraListSet);
            var content = transform.FindDeepChild("Content");
            foreach (var trans in content.GetComponentsInChildren<Transform>(true))
            {
                if (trans.name.ToLower().Contains("data panel"))
                {
                    Destroy(trans.gameObject); // the existing data panels in the system tab we cloned
                }
            }
        }
        /// <summary>
        /// Run once data is populated
        /// </summary>
        public void RunSetup()
        {
            Log.Msg("StreetsUI Panel Setting up");
            
            if (!GetReferences()) { Log.Msg("References failed"); return; }
            ResetMenu();
            SetupPanel();
            SetupSelectors();
            SetupConfigPanel(PanelName);
        }
        public void SetNewDataSet(SmartDataContainerReferenceListSet set,bool keepCurrentTabIndex = true,bool keepCurrentOptionIndex = true)
        {
            listSet = set;
            SetupConfigPanel(PanelName);
        }
        public void SetNewDataList(SmartDataContainerReferenceList list,int indexToChange)
        {

        }
        
        public void OnOpen()
        {
            Log.Msg($"{PanelName} panel opening");
            if (menu.GetMenuSystem() == null)
            {
                Log.Msg($"No menu System found in UIPanel OnOpen, GameObject {PanelName}", true);
                return;
            }
            InputSystemEventCallback inputCancel = GetComponent<InputSystemEventCallback>();
            inputCancel.OnActionPerformed.RemoveAllListeners();
            inputCancel.OnActionPerformedPositive.RemoveAllListeners();
            inputCancel.OnActionPerformed.AddListener(new System.Action(() => { OnClose(); }));

            if (transform.parent.parent != null && transform.parent.parent.name == "Quick Access Menu Canvas")
            {
                var quickInput = transform.parent.parent.gameObject.GetComponent<InputSystemEventCallback>();
                quickInput.OnActionPerformed.AddListener(new System.Action(() => { OnClose(); }));
            }

            OnOpenEvent?.Invoke(config.currentDataIndex);
            menu.OpenMenu();
        }
        public void OnClose()
        {
            Log.Msg($"{PanelName} panel closing");
            OnCloseEvent?.Invoke(config.currentDataIndex);
            InputSystemEventCallback inputCancel = GetComponent<InputSystemEventCallback>();
            inputCancel.OnActionPerformed.RemoveAllListeners();
            inputCancel.OnActionPerformedPositive.RemoveAllListeners();

            if(transform.parent.parent != null && transform.parent.parent.name == "Quick Access Menu Canvas")
            {
                var quickInput = transform.parent.parent.gameObject.GetComponent<InputSystemEventCallback>();
                quickInput.OnActionPerformed.RemoveListener(new System.Action(() => { OnClose(); }));
            }
            else
            {
                gameObject.SetActive(false);
                menu.OpenLastMenu();
            }
        }
        bool GetReferences()
        {
            config = GetComponent<DataConfigPanel>();
            menu = GetComponent<MGMenu>();
            return true;
        }
        public void SetupPanel()
        {
            Log.Msg($"Setup {PanelName}");
            gameObject.name = PanelName;
            foreach (var transform in gameObject.transform)
            {
                if (transform is Transform)
                {
                    var trans = (Transform)transform;
                    if(trans.gameObject != null)
                    {
                        if (trans.gameObject.GetComponent<LocalizeStringEvent>() != null)
                        {
                            trans.gameObject.GetComponent<LocalizeStringEvent>().enabled = false;
                        }
                    }
                }
            }
            menu.isFirstMenu = false;
        }
        public void ResetMenu()
        {
            if (menu == null) { Log.Msg($"No MGmenu"); return; }
            menu.OnEnterOpen = new UnityEvent();
            if (menu.OnOpenRaiseEvents != null) menu.OnOpenRaiseEvents.Clear();
            menu.Awake();
        }
        void SetupSelectors()
        {
            UIHorizontalSelectorSmartSetBehaviour selector = GetComponentInChildren<UIHorizontalSelectorSmartSetBehaviour>(true);
            if (selector.GetComponent<HorizontalSelector>() != null)
            {
                var hSelect = selector.GetComponent<HorizontalSelector>();
                hSelect.enableIndicators = listSet._DataRefLists.Count > 1;
                //hSelect.onValueChanged = new HorizontalSelector.SelectorEvent();
               // hSelect.Awake();
                
            }
            selector._DataReferenceSets = listSet;
            selector._GeneralDataReferenceSets = null;
            selector.OnIndexChanged.RemoveListener(new System.Action<int>(data => OnChangeTab(data)));
            selector.OnIndexChanged.AddListener(new System.Action<int>(data => OnChangeTab(data)));
           // selector.OnIndexChanged = new HorizontalSelectorSmartSetIntCallback(); // maybe hook in here instead of list's OnSelect, list's OnSelect doesnt have a deselect
           // selector.Awake();
        }
        void SetupConfigPanel(string PanelLabel)
        {
            if (config != null)
            {
                Log.Msg("Setting up config panel");
                config.panelLabel.text = PanelLabel;
                config.configDatas = listSet;
                config.configDatas_DevAlt = null;
                config._hasDevAltData = false;
                config._populateOnEnable = true;
                config.useObjectList = false;
                config.currentConfigDatas = null;
                config.currentConfigData = null;
                config.currentDataIndex = 0;
                config._populateOnEnable = true;
                config.OnChangedSelection = new UnityEvent();
                config.OnChangeSelected_Int = new DataConfigPanelIntCallback();
                config._UIHorizontalSelectorSmartSetBehaviour = GetComponentInChildren<UIHorizontalSelectorSmartSetBehaviour>(true);
                config.OnChangeSelected_Int.AddListener(new System.Action<int>((val) => OnChangeTab(val)));
                config._dataSetMenuTree = null;
                config._hasBeenPopulated = false;
                
                config.Start();
                config.Init();
                config.PopulateList();
            }
        }
        void OnChangeTab(int value)
        {
            Debug.Log($"On Change tab {value}");
            OnTabChangedEvent?.Invoke(config.currentDataIndex);
        }
        public void SetPallete(MenuPalette pallete)
        {
            Log.Msg("Setting up pallete");
            config._smartUIManagerData._paletteData.panelColor_01 = pallete.PanelOne;
            config._smartUIManagerData._paletteData.panelColor_02 = pallete.PanelTwo;
            config._smartUIManagerData._paletteData.selectableGraphicColorSet_01 = new UnityEngine.UI.ColorBlock { normalColor = pallete.TextNormal, highlightedColor = pallete.TextHighlighted, pressedColor = pallete.TextSelected };
            config._smartUIManagerData._paletteData.InvokeDataChangedEvent();
            config._smartUIManagerData.InvokePaletteChangeEvent();
        }
       
    }

}