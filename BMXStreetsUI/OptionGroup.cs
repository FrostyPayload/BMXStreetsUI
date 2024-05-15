using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Just a package of options with a title attached, for sending to StreetsUI when building
    /// </summary>
    public class OptionGroup
    {
        /// <summary>
        /// The title on the menu when it opens
        /// </summary>
        public string title { get;private set; }
        /// <summary>
        /// Sliders, Toggles, Buttons and SteppedInt's all go in here
        /// </summary>
        public List<OptionBase> options;
        public OptionGroup(string GroupTitle)
        {
            this.title = GroupTitle;
            options = new List<OptionBase>();
        }
       
    }
}
