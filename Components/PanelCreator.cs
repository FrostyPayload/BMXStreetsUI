using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BmxStreetsUI.Components.GrindPosePanel;
using UnityEngine.Events;

namespace BmxStreetsUI.Components
{
    public class PanelCreator
    {
        public virtual bool Create(CustomMenu newmenu)
        {
            GameObject systemSettingsPanel = GameObject.Find("System Tab Settings");
            var systemSettingsTab = GameObject.Find("SYSTEM-TAB");
            if (systemSettingsPanel == null) // does it need to be active for .Find()
            {
                foreach (var menu in UnityEngine.Object.FindObjectsByType<MGMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (menu.gameObject.name.ToLower().Contains("system tab settings"))
                    {
                        systemSettingsPanel = menu.gameObject;
                        break;
                    }
                }
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
                var NewTab = duplicateSettingPanel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                NewTab.tab = duplicateTab;
                NewTab.tabParent = systemSettingsTab.transform.parent.gameObject;
                SetupData(NewTab,newmenu);
                NewTab.RunSetup();
                return true;
            }
            Log.Msg($"Failed to create {newmenu.panelTitle} UI");
            return false;
        }
        public virtual void SetupData(UIPanel panel, CustomMenu menu)
        {
            Log.Msg($"Setting up {menu.panelTitle} UI Data");
            var listSet = ScriptableObject.CreateInstance<SmartDataContainerReferenceListSet>();
            if (listSet == null) { Log.Msg($"ListSet is null in {menu.panelTitle} setup", true); return; }
            panel.TabName = menu.panelTitle;

            // populate list with every custommenugroup, setting up each options data and callabacks
            listSet._DataRefLists = new Il2CppSystem.Collections.Generic.List<SmartDataContainerReferenceList>();
            foreach (var tab in menu.Groups)
            {
                var GroupOptions = ScriptableObject.CreateInstance<SmartDataContainerReferenceList>();
                GroupOptions.ListName = tab.title;
                GroupOptions.name = tab.title + "ReferenceList";
                GroupOptions.OnSelected = new UnityEvent();
                GroupOptions.OnValueChanged = new UnityEvent();
                GroupOptions.OnDataChangeableChanged = new UnityEvent();
                GroupOptions.OnValueChanged_DataValue = new SmartDataEvent();

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

            listSet.SetName = menu.panelTitle + "ReferenceListSet";
            listSet.name = listSet.SetName + "Object";
            panel.listSet = listSet;
        }
        
        void SetupOption(CustomMenuOption option, SmartDataContainer container)
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
