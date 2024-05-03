using BmxStreetsUI.Components;
using Il2CppInterop.Runtime;

namespace BmxStreetsUI
{
    public class Toggle : MenuOptionBase
    {
        Action<bool> boolCallback;
        public Toggle(string title, string description = "",bool defaultValue = false) : base(title, description, defaultValue == true ? 1 : 0)
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
            base.OnCallBackValue(obj);
            Log.Msg($"Toggle callback received");
            if (obj.GetIl2CppType() == Il2CppType.Of<float>())
            {
                float value = obj.Unbox<float>();
                boolCallback?.Invoke(value>0);
            }
        }

        /// <summary>
        /// The action to be called whenever this value is changed, is also called when the underlying data receives loaded values
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallBack(Action<bool> callback)
        {
            this.boolCallback = callback;
        }

    }
}
