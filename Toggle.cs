using BmxStreetsUI.Components;
using Il2CppInterop.Runtime;

namespace BmxStreetsUI
{
    public class Toggle : CustomMenuOption
    {
        Action<bool> boolCallback;
        public Toggle(string title, string description = "") : base(title, description)
        {
            SetUIStyle(UIStyle.Toggle);
        }
        public override Il2CppSystem.Collections.Generic.List<string> GetLabels()
        {
            var list = new Il2CppSystem.Collections.Generic.List<string>();
            list.Add("Off");
            list.Add("On");
            return list;
        }
        protected override void OnCallBackValue(Il2CppSystem.Object obj)
        {
            Log.Msg($"Toggle callback received with type {obj.GetIl2CppType().ToString()}");
            if (obj.GetIl2CppType() == Il2CppType.Of<float>())
            {
                float value = BitConverter.ToSingle(BitConverter.GetBytes(obj.Pointer.ToInt64()));
                boolCallback?.Invoke(value>0);
            }
        }
        public void SetCallBack(Action<bool> callback)
        {
            this.boolCallback = callback;
        }

    }
}
