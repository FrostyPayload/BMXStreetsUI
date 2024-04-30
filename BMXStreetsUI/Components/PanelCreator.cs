using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI.Components
{
    public class PanelCreator
    {
        public PanelCreator(GameObject tab, GameObject panel)
        {
            this.systemSettingsTab = tab;
            this.systemSettingsPanel = panel;
        }
        GameObject systemSettingsPanel, systemSettingsTab;
        GameObject? tab, Panel;
        public virtual bool Create(CustomMenu newmenu)
        {
            if (systemSettingsPanel != null && systemSettingsTab != null)
            {
                Log.Msg($"Setting up new StreetsUI");
                 tab = UnityEngine.Object.Instantiate(systemSettingsTab);
                 Panel = UnityEngine.Object.Instantiate(systemSettingsPanel);
                if (tab == null)
                {
                    Log.Msg("Failed to find the system tab for duplication");
                    OnFail(newmenu.TabTitle);
                    return false;
                }
                if (Panel == null)
                {
                    Log.Msg("failed to find the system panel for duplication");
                    OnFail(newmenu.TabTitle);
                    return false;
                }
                tab.transform.SetParent(systemSettingsTab.transform.parent, false);
                tab.GetComponent<LocalizeStringEvent>().enabled = false;
                tab.GetComponentInChildren<TextMeshProUGUI>().text = newmenu.TabTitle;
                tab.name = newmenu.TabTitle + "Object";

                var smartui = tab.GetComponent<SmartUIBehaviour>();
                smartui.UnRegisterEvents();

                var UIPanel = Panel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                UIPanel.Init();
                UIPanel.transform.SetParent(systemSettingsPanel.transform.parent, false);
                UIPanel.SetupTriggers(tab);
                SetupData(UIPanel,newmenu);
                UIPanel.RunSetup();
                newmenu.panel = UIPanel;
                
                if(newmenu.overridePallete) UIPanel.SetPallete(newmenu.pallete);
                
                return true;
            }
            OnFail(newmenu.TabTitle);
            return false;
        }
       
        void OnFail(string menuname)
        {
            Log.Msg($"Failed to Create {menuname}");
            if(tab != null) UnityEngine.GameObject.Destroy(tab);
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
    }
}
