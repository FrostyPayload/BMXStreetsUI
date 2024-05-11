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
        /// The monoBehaviour on your panel once created
        /// </summary>
        public UIPanel? panel;
        public MenuPalette palette;
        public bool overridePallete;
        
        public Action<int>? OnMenuOpen;
        public Action<int>? OnMenuClose;
        public Action<int>? OnTabChange;
        /// <summary>
        /// no support yet
        /// </summary>
        public Action<int>? OnSelectionChange;
        public Action? OnSave;
        public Action? OnLoad;

        /// <summary>
        /// loads data from locallow/Mash/Containers/{steamId}/{TabTitle}/{TabTitle}.container. Creates directory and file as needed, as each loaded value is matched with data in your menu, the options callback will fire with the loaded value.
        /// In other words, calling load will automatically apply all changes if your receiving code is arranged accordingly.
        /// </summary>
        public void Load()
        {
            if (panel!=null && panel.listSet!=null)
            {
                foreach(var list in panel.listSet._DataRefLists)
                {
                    list.Load();
                }
            }
            OnLoad?.Invoke();
        }
        public void Save()
        {
            if (panel != null && panel.listSet!= null)
            {
                foreach (var list in panel.listSet._DataRefLists)
                {
                    list.Save();
                }
            }
            OnSave?.Invoke();
        }
        internal void MenuOpenedEvent(int tab)
        {
            OnMenuOpen?.Invoke(tab);
        }
        internal void MenuClosedEvent(int tab)
        {
            OnMenuClose?.Invoke(tab);
        }
        internal void TabChangedEvent(int tab)
        {
            OnTabChange?.Invoke(tab);
        }
        
        /// <summary>
        ///  Once the panel is setup, attempts to retreive the active groups data.
        /// </summary>
        /// <returns></returns>
        public SmartDataContainerReferenceList? GetCurrentDataList()
        {
            return panel!= null && panel.listSet != null && panel.config.currentConfigData != null ? panel.config.currentConfigData : null;
        }
        /// <summary>
        /// Once the panel is active, attempts to retreive the whole dataset.
        /// </summary>
        /// <returns></returns>
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
