using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Create an instance of this with your list of groups and a tab name, then register it with API.AddMenu();
    /// </summary>
    public class CustomMenu
    {
        /// <summary>
        /// The title that appears on the main menu
        /// </summary>
        public string TabTitle;
        /// <summary>
        /// Each group you place in here shows up as a tab of options inside your menu
        /// </summary>
        public List<CustomMenuOptionGroup> Groups;
        /// <summary>
        /// The monoBehaviour on your panel
        /// </summary>
        public UIPanel? panel;
        public CustomMenuPallete pallete;
        public bool overridePallete;
        Action<int>? OnTabChange;
        public bool AutoLoad = true;
        public bool AutoSave = true;
        /// <summary>
        /// The funtion to run as soon as the Menu is created.
        /// </summary>
        public Action<CustomMenu>? OnMenuCreation;
        

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
        public void SetOnOpenCallback(Action<int> OnMenuOpened)
        {
            if (panel)
            {
                panel.OnOpenEvent.RemoveListener(OnMenuOpened);
                panel.OnOpenEvent.AddListener(OnMenuOpened);
            }
        }
        public void SetOnCloseCallback(Action<int> OnMenuClosed)
        {
            if (panel)
            {
                panel.OnOpenEvent.RemoveListener(OnMenuClosed);
                panel.OnOpenEvent.AddListener(OnMenuClosed);
            }
        }
        public void SetOnGroupChangedCallback(Action<int> OnTabChanged)
        {
            if (panel)
            {

            }
        }
        public CustomMenu(string title, List<CustomMenuOptionGroup> groups) 
        { 
            this.TabTitle = title;
            this.Groups = groups;
        }

        public void UpdateOptions(List<CustomMenuOptionGroup> options)
        {

        }
    }
}
