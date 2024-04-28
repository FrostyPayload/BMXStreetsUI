using BmxStreetsUI.Components;
using Il2CppInterop.Runtime;

namespace BmxStreetsUI
{
    /// <summary>
    /// You provide a list of strings to display, the index of the selection is sent back to you on change of the value;
    /// </summary>
    public class SteppedInt : CustomMenuOption
    {
        Action<int> IntCallback;
        public Il2CppSystem.Collections.Generic.List<string> choices;
        public SteppedInt(string title, string description = "") : base(title, description)
        {
            SetUIStyle(UIStyle.SteppedInt);
            choices = new Il2CppSystem.Collections.Generic.List<string>();
        }
        protected override void OnCallBackValue(Il2CppSystem.Object obj)
        {
            Log.Msg($"SteppedInt callback received with type {obj.GetIl2CppType().ToString()}");
            if (obj.GetIl2CppType() == Il2CppType.Of<float>())
            {
                float value = BitConverter.ToSingle(BitConverter.GetBytes(obj.Pointer.ToInt64()));
                IntCallback?.Invoke((int)value);
            }
        }
        public void SetCallBack(Action<int> callback)
        {
            this.IntCallback = callback;
        }
        public override Il2CppSystem.Collections.Generic.List<string> GetLabels()
        {
            return choices;
        }
    }

}
