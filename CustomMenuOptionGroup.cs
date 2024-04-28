using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmxStreetsUI
{
    /// <summary>
    /// Represents a selection of options in the UI, such as Gameplay, Audio etc
    /// </summary>
    public class CustomMenuOptionGroup
    {
        /// <summary>
        /// The title on the menu when it opens
        /// </summary>
        public string title;
        /// <summary>
        /// An entry in a tab, like a slider or button
        /// </summary>
        public List<CustomMenuOption> options;

        public CustomMenuOptionGroup(string title)
        {
            this.title = title;
            options = new List<CustomMenuOption>();
        }
    }
}
