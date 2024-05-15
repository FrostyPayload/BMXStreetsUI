using BmxStreetsUI.Components;
using Il2CppInterop.Runtime;

namespace BmxStreetsUI
{
    /// <summary>
    /// You provide a list of strings to display, the index of the selection is sent back to you on change of the value;
    /// </summary>
    public class SteppedInt : OptionBase
    {
        Action<int>? IntCallback;
        public Il2CppSystem.Collections.Generic.List<string> choices;
        public SteppedInt(string title, string description = "",float defaultIndex = 0) : base(title, description,defaultIndex)
        {
            SetUIStyle(UIStyle.SteppedInt);
            choices = new Il2CppSystem.Collections.Generic.List<string>();
        }
        internal override void OnCallBackValue(Il2CppSystem.Object obj)
        {
            base.OnCallBackValue(obj);
            //Log.Msg($"SteppedInt callback received");
            if (obj.GetIl2CppType() == Il2CppType.Of<float>())
            {
                float value = obj.Unbox<float>();
                IntCallback?.Invoke((int)value);
            }
        }

        /// <summary>
        /// The action to be called whenever this value is changed, is also called when the underlying data receives loaded values
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallBack(Action<int> callback)
        {
            this.IntCallback = callback;
        }
        internal override Il2CppSystem.Collections.Generic.List<string> GetLabels()
        {
            return choices;
        }
    }

}
