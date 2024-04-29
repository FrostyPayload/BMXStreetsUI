
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
