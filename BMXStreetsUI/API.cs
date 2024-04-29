using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class API
    {
        internal static bool Initialize()
        {
            Log.Msg($"API Initialize");
            return true;
        }
        /// <summary>
        /// Create a CustomMenu and use the other classes in this namespace to setup your menu, then bring it here to have it created and setup
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns>Did the creation succeed. On fail, check unity logs for info</returns>
        public static bool AddMenu(CustomMenu newMenu)
        {
            var Creator = new PanelCreator();
            return Creator.Create(newMenu);
        }
        /// <summary>
        /// Retreive a menu using the name set in CustomMenu.panelTitle
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns>The menu if found, or null otherwise</returns>
        public static CustomMenu? GetMenu(string menuName)
        {
            return null;
        }
        public static bool RemoveMenu(CustomMenu menu)
        {
            return false;
        }

    }

   

}
