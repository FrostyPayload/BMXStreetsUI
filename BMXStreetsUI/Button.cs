using BmxStreetsUI.Components;

namespace BmxStreetsUI
{
    public class Button : MenuOptionBase
    {
        public Button(string title, string description = "") : base(title, description)
        {
            SetUIStyle(UIStyle.Button);
        }
        /// <summary>
        /// The function to run when the button is pressed 
        /// </summary>
        /// <param name="voidCallBack"></param>
        public void SetCallBack(Action voidCallBack)
        {
            this.VoidCallBack = voidCallBack;
        }
    }
}
