
using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Represents a selection of options in the UI, such as the Gameplay or Audio option set.
    /// Requires a title and your list of options.
    /// </summary>
    public class CustomMenuOptionGroup
    {
        /// <summary>
        /// The title on the menu when it opens
        /// </summary>
        public string title { get;private set; }
        /// <summary>
        /// Sliders, Toggles, Buttons and SteppedInt's all go in here
        /// </summary>
        public List<CustomMenuOption> options;

        public CustomMenuOptionGroup(string GroupTitle)
        {
            this.title = GroupTitle;
            options = new List<CustomMenuOption>();
        }
    }
}
