using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmxStreetsUI
{
    /// <summary>
    /// The base class which sliders,toggles,buttons inherit from.
    /// </summary>
    public abstract class CustomMenuOption
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

        Action<object> callback;
        public bool SetCallBack(Action<object> callback)
        {
            this.callback = callback;
            return false;
        }
        public virtual float GetMax()
        {
            return 1;
        }
        public virtual float GetMin()
        {
            return 0;
        }

    }


}
