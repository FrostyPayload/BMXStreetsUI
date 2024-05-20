using Il2Cpp;

namespace BmxStreetsUI.Components
{
    /// <summary>
    /// The base class which sliders,toggles,buttons,steppedInt's inherit from.
    /// </summary>
    public abstract class OptionBase
    {
        public OptionBase(string title, string description = "",float defaultValue = 0)
        {
            this.title = title;
            this.description = description;
            this.defaultValue = defaultValue;
            VoidCallBack = OnCallBack;
            ValueCallBack = OnCallBackValue;
        }
        /// <summary>
        /// name of the option seen on the left of the value your editing
        /// </summary>
        public string title;
        /// <summary>
        /// Description that pops up on the right as you edit a value, the presence of any data here makes the panel show up
        /// </summary>
        public string description;
        public float defaultValue;
        /// <summary>
        /// Take a look at the static members of the DataUnits class in the game's codebase. This string determines the measurement displayed next to your value
        /// but also defines how the value is displayed. for example, a value here of "m" indicating metres causing the decimal places to be displayed.
        /// </summary>
        public string DataUnit;
        internal UIStyle UIStyle { get; private set; }
        /// <summary>
        /// the button just resets this instead of having its own
        /// </summary>
        internal Action VoidCallBack { get; set; }
        /// <summary>
        /// derived classes override the target of this
        /// </summary>
        internal Action<Il2CppSystem.Object> ValueCallBack { get; private set; }
       
        internal void SetUIStyle(UIStyle style)
        {
            UIStyle = style;
        }

        /// <summary>
        /// Derived classes override this to provide more specific objects to their listeners.
        /// </summary>
        /// <param name="callback"></param>
        internal virtual void OnCallBackValue(Il2CppSystem.Object obj)
        {
           // Log.Msg($"OnCallBackValue received with type {obj.GetIl2CppType().ToString()}");
        }
        internal virtual void OnCallBack()
        {
           // Log.Msg($"OnCallBack received");
        }
        internal virtual float GetMax()
        {
            return 1.00f;
        }
        internal virtual float GetMin()
        {
            return 0.00f;
        }
        /// <summary>
        /// Toggles and steppedInt's are powered by this list,max and min are set by the count. Toggles simply give back on and off labels and return 0 or 1
        /// </summary>
        /// <returns></returns>
        internal virtual Il2CppSystem.Collections.Generic.List<string> GetLabels()
        {
            return new Il2CppSystem.Collections.Generic.List<string>();
        }

    }
    public enum UIStyle { Button, Toggle, Slider, SteppedInt }

}
