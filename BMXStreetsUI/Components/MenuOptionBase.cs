namespace BmxStreetsUI.Components
{
    /// <summary>
    /// The base class which sliders,toggles,buttons,steppedInt's inherit from.
    /// </summary>
    public abstract class MenuOptionBase
    {
        public MenuOptionBase(string title, string description = "",float defaultValue = 0)
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
        public int decimalPlaces = 1;
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
        internal Action SelectCallBack { get; private set; }
        internal Action DeSelectCallBack { get; private set; }
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
            Log.Msg($"OnCallBackValue received with type {obj.GetIl2CppType().ToString()}");
        }
        internal virtual void OnCallBack()
        {
            Log.Msg($"OnCallBack received");
        }
        internal virtual float GetMax()
        {
            return 1;
        }
        internal virtual float GetMin()
        {
            return 0;
        }
        /// <summary>
        /// Toggles and steppedInt's are powered by this list,max and min are set by the count. Toggles simply give back on and off labels and return 0 or 1
        /// </summary>
        /// <returns></returns>
        internal virtual Il2CppSystem.Collections.Generic.List<string> GetLabels()
        {
            return new Il2CppSystem.Collections.Generic.List<string>();
        }
        /// <summary>
        /// Set a callback to happen when this option becomes the highlighted selection in the UI
        /// </summary>
        /// <param name="actionWhenSelected"></param>
        public void SetOnSelected(Action actionWhenSelected)
        {
            this.SelectCallBack = actionWhenSelected;
        }
        /// <summary>
        /// Set a callback for when this option stops being the chosen selection in the UI
        /// </summary>
        /// <param name="actionWhenDeSelected"></param>
        public void SetOnDeSelected(Action actionWhenDeSelected)
        {
            this.DeSelectCallBack = actionWhenDeSelected;
        }
    }
    public enum UIStyle { Button, Toggle, Slider, SteppedInt }

}
