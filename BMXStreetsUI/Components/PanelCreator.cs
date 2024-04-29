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
        GameObject? tab, Panel;
        public virtual bool Create(CustomMenu newmenu)
        {
            GameObject systemSettingsPanel = GameObject.Find(Constants.SystemTabSettings);
            var systemSettingsTab = GameObject.Find("SYSTEM-TAB");
            if (systemSettingsPanel == null) // does it need to be active for .Find()
            {
                systemSettingsPanel = UnityEngine.Object.FindObjectsByType<MGMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                                                        .Where(menu => menu.gameObject.name.ToLower()
                                                                                           .Contains(Constants.SystemTabSettings))
                                                        .FirstOrDefault().gameObject;
            }

            if (systemSettingsPanel != null && systemSettingsTab != null)
            {
                Log.Msg($"Setting up new StreetsUI");
                 tab = UnityEngine.Object.Instantiate(systemSettingsTab);
                 Panel = UnityEngine.Object.Instantiate(systemSettingsPanel);
                if (tab == null)
                {
                    Log.Msg("Failed to find the system tab for duplication");
                    OnFail();
                    return false;
                }
                if (Panel == null)
                {
                    Log.Msg("failed to find the system panel for duplication");
                    OnFail();
                    return false;
                }
                tab.transform.SetParent(systemSettingsTab.transform.parent, false);
                tab.GetComponent<LocalizeStringEvent>().enabled = false;
                tab.GetComponentInChildren<TextMeshProUGUI>().text = newmenu.TabTitle;
                tab.name = newmenu.TabTitle + "Object";

                var smartui = tab.GetComponent<SmartUIBehaviour>();
                smartui.UnRegisterEvents();

                var UIPanel = Panel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                UIPanel.transform.SetParent(systemSettingsPanel.transform.parent, false);
                UIPanel.SetupTriggers(tab);
                SetupData(UIPanel,newmenu);
                UIPanel.RunSetup();
                return true;
            }
            Log.Msg($"Failed to create {newmenu.TabTitle} UI");
            OnFail();
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
            foreach (var tab in menu.Groups)
            {
                var GroupOptions = SmartDataManager.CreateNewList(tab.title);
                var container = SmartDataManager.GetNewContainer(tab.title,menu.TabTitle);

                foreach(var option in tab.options)
                {
                    SetupOption(option, container);
                }
                GroupOptions.connectedContainer = container;
                GroupOptions._dataContainer = container;
                GroupOptions.dataGroup = container._smartDatas;
                container.ValidateList();
                listSet._DataRefLists.Add(GroupOptions);
            }
            panel.PanelName = menu.TabTitle;
            panel.listSet = listSet;
        }
        
        protected virtual void SetupOption(CustomMenuOption option, SmartDataContainer container)
        {
            var data = SmartDataManager.CreateSmartData(option);
            data.OnDataChangeableChanged = new UnityEvent();
            data.DataChangeableCallback = new BoolCallBackEvent();
            data.OnValueChanged_DataValue = new SmartDataEvent();
            data.OnValueChanged_DataValue.AddListener(option.ValueCallBack);
            data.OnValueChanged = new UnityEvent();
            data.OnValueChanged.AddListener(option.VoidCallBack);
            data.EnableDataChangeable();
            container._smartDatas.Add(data);
        }
    }
}
