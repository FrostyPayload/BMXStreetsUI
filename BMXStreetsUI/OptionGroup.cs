
using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    /// <summary>
    /// Represents a selection of options you want in your UI, like the Gameplay or Audio option set's in the system menu.
    /// Requires a title and your list of options.
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
        public List<MenuOptionBase> options;
        internal Action SelectCallBack { get; private set; }
        internal Action DeSelectCallBack { get; private set; }
        public OptionGroup(string GroupTitle)
        {
            this.title = GroupTitle;
            options = new List<MenuOptionBase>();
        }
        public void SetOnSelected(Action OnTabSelected)
        {
            this.SelectCallBack = OnTabSelected;
        }
        public void SetOnDeSelected(Action OnTabDeSelected)
        {
            this.DeSelectCallBack = OnTabDeSelected;
        }
    }
}
