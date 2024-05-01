using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI.Components
{
    public class UICreator
    {
        GameObject? Panel;
        public virtual GameObject? CreatePanel(CustomMenu newmenu, GameObject systemSettingsPanel,bool parentToMainRoot = true, bool RunSetup = true)
        {
            if (systemSettingsPanel != null)
            {
                Log.Msg($"Setting up new StreetsUI");
                 Panel = UnityEngine.Object.Instantiate(systemSettingsPanel);
                if (Panel == null)
                {
                    Log.Msg("failed to find the system panel for duplication");
                    OnFail(newmenu.TabTitle);
                    return null;
                }
                var UIPanel = Panel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                UIPanel.Init();
                if(parentToMainRoot) UIPanel.transform.SetParent(systemSettingsPanel.transform.parent, false);
                SetupData(UIPanel,newmenu);
                if(RunSetup) UIPanel.RunSetup();
                newmenu.panel = UIPanel;
                
                if(newmenu.overridePallete) UIPanel.SetPallete(newmenu.pallete);
                
                return Panel;
            }
            OnFail(newmenu.TabTitle);
            return null;
        }
        
        void OnFail(string menuname)
        {
            Log.Msg($"Failed to Create {menuname}");
            if (Panel != null) UnityEngine.GameObject.Destroy(Panel);
        }
        protected virtual void SetupData(UIPanel panel, CustomMenu menu)
        {
            Log.Msg($"Setting up {menu.TabTitle} UI Data");
            var listSet = SmartDataManager.CreateNewSet(menu.TabTitle);
            if (listSet == null) { Log.Msg($"ListSet is null in {menu.TabTitle} setup", true); return; }
            
            // populate list with every custommenugroup, setting up each options data and callbacks
            listSet._DataRefLists = new Il2CppSystem.Collections.Generic.List<SmartDataContainerReferenceList>();
            foreach (var group in menu.Groups)
            {
                var GroupSmartList = SmartDataManager.CreateNewList(group.title);
                var container = SmartDataManager.CreateNewContainer(group.title,menu.TabTitle);

                foreach(var option in group.options)
                {
                    SetupOption(option, container);
                }
                GroupSmartList.connectedContainer = container;
                GroupSmartList._dataContainer = container;
                GroupSmartList.dataGroup = container._smartDatas;
                container.ValidateList();
                GroupSmartList.OnSelected = new UnityEvent();
                if(group.SelectCallBack != null) GroupSmartList.OnSelected.AddListener(group.SelectCallBack);
                listSet._DataRefLists.Add(GroupSmartList);
            }
            panel.PanelName = menu.TabTitle;
            panel.listSet = listSet;

        }
        
        protected virtual void SetupOption(CustomMenuOption option, SmartDataContainer container)
        {
            var data = SmartDataManager.CreateSmartData(option);
            data.OnValueChanged_DataValue.AddListener(option.ValueCallBack);
            data.OnValueChanged.AddListener(option.VoidCallBack);
            data.EnableDataChangeable();
            container._smartDatas.Add(data);
        }

        public virtual GameObject? CreateTab(GameObject settingsTab, string TabLabel, bool parentToMain = true)
        {
            var tab = UnityEngine.Object.Instantiate(settingsTab);
            if (parentToMain) tab.transform.SetParent(settingsTab.transform.parent, false);
            tab.GetComponent<LocalizeStringEvent>().enabled = false;
            tab.GetComponentInChildren<TextMeshProUGUI>().text = TabLabel;
            tab.name = TabLabel + "Object";
            var smartui = tab.GetComponent<SmartUIBehaviour>();
            smartui.UnRegisterEvents();

            return tab;
        }
        
        public virtual GameObject CreateTab(string TabLabel, GameObject systemSettingsTab,Transform parent = null)
        {
            var toParent = parent == null ? systemSettingsTab.transform.parent : parent;
            var tab = UnityEngine.Object.Instantiate(systemSettingsTab);
            tab.transform.SetParent(toParent, false);
            tab.GetComponent<LocalizeStringEvent>().enabled = false;
            tab.GetComponentInChildren<TextMeshProUGUI>().text = TabLabel;
            tab.name = TabLabel + "Object";
            var smartui = tab.GetComponent<SmartUIBehaviour>();
            smartui.UnRegisterEvents();

            return tab;
        }
    }
}
