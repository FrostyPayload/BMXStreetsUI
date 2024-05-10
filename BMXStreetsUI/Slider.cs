using BmxStreetsUI.Components;
using Il2CppInterop.Runtime;

namespace BmxStreetsUI
{
    /// <summary>
    /// Give a name, max and min value and callback. Calls back with value on change
    /// </summary>
    public class Slider : MenuOptionBase
    {
        Action<float> floatcallback;
        public float max, min;
        public Slider(string title, float min, float max, string description = "", float defaultValue = 0) : base(title, description,defaultValue)
        {
            SetUIStyle(UIStyle.Slider);
            this.min = min;
            this.max = max;
            
        }
        internal override float GetMax()
        {
            return max;
        }
        internal override float GetMin()
        {
            return min;
        }
        internal override void OnCallBackValue(Il2CppSystem.Object obj)
        {
            base.OnCallBackValue(obj);
            //Log.Msg("Slider callback received");
            if (obj.GetIl2CppType() == Il2CppType.Of<float>())
            {
                float value = obj.Unbox<float>();
                floatcallback?.Invoke(value);
            }
        }

        /// <summary>
        /// The action to be called whenever this value is changed, is also called when the underlying data receives loaded values
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallBack(Action<float> callback)
        {
            this.floatcallback = callback;
        }
    }
}
