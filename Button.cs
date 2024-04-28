
using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    public class Button : CustomMenuOption
    {
        public Button(string title, string description = "") : base(title, description)
        {
            SetUIStyle(UIStyle.Button);
        }
        public void SetCallBack(Action voidCallBack)
        {
            this.VoidCallBack = voidCallBack;
        }
    }
}
