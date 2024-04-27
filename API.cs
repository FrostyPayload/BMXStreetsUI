using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmxStreetsUI
{
    /// <summary>
    /// Modders can use this API to easily add custom UI to the game and get callbacks to their own code
    /// </summary>
    public static class API
    {
        public static CustomMenu AddCustomTabToMainMenu()
        {
            return new CustomMenu();
        }
        public static void RemoveMenu(CustomMenu menu)
        {

        }

    }

    public class CustomMenu
    {
        /// <summary>
        /// The title that appears on the main menu
        /// </summary>
        public string panelTitle;
        /// <summary>
        /// Each group shows up as a tab inside your menu
        /// </summary>
        public List<CustomMenuGroup> Groups;

        public void UpdateOptions(List<CustomMenuGroup> options)
        {

        }
    }


    /// <summary>
    /// Represents a selection of options in the UI, such as Gameplay, Audio etc
    /// </summary>
    public class CustomMenuGroup
    {
        /// <summary>
        /// The title on the menu when it opens
        /// </summary>
        public string title;
        public List<CustomMenuOption> options;
    }


    /// <summary>
    /// An option in a tab, such as a slider, button, incremented value
    /// </summary>
    public class CustomMenuOption
    {
        /// <summary>
        /// Title seen on the left of the value your editing
        /// </summary>
        public string title;
        /// <summary>
        /// Description that pops up on the right as you edit a value
        /// </summary>
        public string description;
        public bool DescriptionShouldShow;
        public SmartData.DataUIStyle uiStyle;
    }

}
