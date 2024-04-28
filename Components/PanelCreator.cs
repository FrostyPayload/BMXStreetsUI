using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using Il2CppTMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

namespace BmxStreetsUI.Components
{
    public class PanelCreator
    {
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
                Log.Msg($"Setting up new UI");
                var duplicateTab = UnityEngine.Object.Instantiate(systemSettingsTab);
                var duplicateSettingPanel = UnityEngine.Object.Instantiate(systemSettingsPanel);
                if (duplicateTab == null)
                {
                    Log.Msg("Failed to find the system tab for duplication");
                    return false;
                }
                if (duplicateSettingPanel == null)
                {
                    Log.Msg("failed to find the system panel for duplication");
                    return false;
                }
                duplicateTab.transform.SetParent(systemSettingsTab.transform.parent, false);
                duplicateTab.GetComponent<LocalizeStringEvent>().enabled = false;
                duplicateTab.GetComponentInChildren<TextMeshProUGUI>().text = newmenu.panelTitle;
                duplicateTab.name = newmenu.panelTitle + "Object";

                var smartui = duplicateTab.GetComponent<SmartUIBehaviour>();
                smartui.UnRegisterEvents();

                var UIPanel = duplicateSettingPanel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                UIPanel.transform.SetParent(systemSettingsPanel.transform.parent, false);
                UIPanel.SetupTriggers(duplicateTab);
                SetupData(UIPanel,newmenu);
                UIPanel.RunSetup();
                return true;
            }
            Log.Msg($"Failed to create {newmenu.panelTitle} UI");
            return false;
        }

        public virtual void SetupData(UIPanel panel, CustomMenu menu)
        {
            Log.Msg($"Setting up {menu.panelTitle} UI Data");
            var listSet = SmartDataManager.CreateNewSet(menu.panelTitle);
            if (listSet == null) { Log.Msg($"ListSet is null in {menu.panelTitle} setup", true); return; }
            
            // populate list with every custommenugroup, setting up each options data and callbacks
            listSet._DataRefLists = new Il2CppSystem.Collections.Generic.List<SmartDataContainerReferenceList>();
            foreach (var tab in menu.Groups)
            {
                var GroupOptions = SmartDataManager.CreateNewList(tab.title);
                var container = SmartDataManager.GetNewContainer(tab.title);

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
            panel.PanelName = menu.panelTitle;
            panel.listSet = listSet;
        }
        
        public virtual void SetupOption(CustomMenuOption option, SmartDataContainer container)
        {
            var data = SmartDataManager.CreateSmartDatas(option);
            data._dataUIStyle = option.uiStyle;
            data.OnDataChangeableChanged = new UnityEvent();
            data.DataChangeableCallback = new BoolCallBackEvent();
            data.OnValueChanged_DataValue = new SmartDataEvent();
            data.OnValueChanged_DataValue.AddListener(option.callback);
            data.OnValueChanged = new UnityEvent();
           // data.OnValueChanged.AddListener(option.voidCallBack);
            data.EnableDataChangeable();
            container._smartDatas.Add(data);
        }
    }
}
