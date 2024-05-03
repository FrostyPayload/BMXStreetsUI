using BmxStreetsUI.Components;
using Il2Cpp;

namespace BmxStreetsUI
{
    /// <summary>
    /// Create an instance of this with your list of groups and a tab name, then register it with API.AddMenu();
    /// </summary>
    public class MenuPanel
    {
        /// <summary>
        /// The title that appears on the main menu
        /// </summary>
        public string TabTitle;
        /// <summary>
        /// Each group you place in here shows up as a tab of options inside your menu
        /// </summary>
        public List<OptionGroup> Groups;
        /// <summary>
        /// The monoBehaviour on your panel
        /// </summary>
        public UIPanel? panel;
        public MenuPalette palette;
        public bool overridePallete;
        
        public Action<int>? OnMenuOpen;
        public Action<int>? OnMenuClose;
        public Action<int>? OnTabChange;
        public Action<int>? OnSelectionChange;

        /// <summary>
        /// loads data from locallow/Mash/Containers/{steamId}/{TabTitle}/{TabTitle}.container. Creates directory and file as needed, as each loaded value is matched with data in your menu, the options callback will fire with the loaded value.
        /// In other words, calling load will automatically apply all changes if your receiving code is arranged accordingly.
        /// </summary>
        public void Load()
        {
            if (panel && panel.listSet)
            {
                foreach(var list in panel.listSet._DataRefLists)
                {
                    list.Load();
                }
            }
        }
        public void Save()
        {
            if (panel && panel.listSet)
            {
                foreach (var list in panel.listSet._DataRefLists)
                {
                    list.Save();
                }
            }
        }
        public void SetPanelOnOpenCallback(Action<int> OnMenuOpened)
        {
            if (panel)
            {
                panel.OnOpenEvent.RemoveListener(OnMenuOpened);
                panel.OnOpenEvent.AddListener(OnMenuOpened);
            }
        }
        public void SetPanelOnCloseCallback(Action<int> OnMenuClosed)
        {
            if (panel)
            {
                panel.OnCloseEvent.RemoveListener(OnMenuClosed);
                panel.OnCloseEvent.AddListener(OnMenuClosed);
            }
        }
        public void SetPanelOnTabChangedCallback(Action<int> Action)
        {
            if (panel)
            {
                panel.OnTabChanged.RemoveListener(Action);
                panel.OnTabChanged.AddListener(Action);
            }
        }
        public void SetPanelOnSelectionChangedCallback(Action<int> Action)
        {
            if (panel)
            {
                panel.OnSelectionChanged.RemoveListener(Action);
                panel.OnSelectionChanged.AddListener(Action);
            }
        }

        public void SwapLiveGroup(OptionGroup newGroup)
        {

        }

        public SmartDataContainerReferenceList? GetCurrentDataList()
        {
            return panel!= null && panel.listSet != null && panel.config.currentConfigData != null ? panel.config.currentConfigData : null;
        }
        public SmartDataContainerReferenceListSet? GetData()
        {
            return panel != null && panel.listSet != null && panel.config.currentConfigDatas != null ? panel.config.currentConfigDatas : null;
        }
        public MenuPanel(string title, List<OptionGroup> groups) 
        { 
            this.TabTitle = title;
            this.Groups = groups;
        }

    }
}
