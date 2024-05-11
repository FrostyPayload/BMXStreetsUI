using MelonLoader;
using BmxStreetsUI;
using UnityEngine;
using Il2CppMG_UI.MenuSytem;

[assembly: MelonInfo(typeof(BMXStreetsUIExampleMod.ExampleUIMelon),"BMXStreetsUIExampleMod", "1.0.0", "FrostyP/LineRyder")]
[assembly: MelonGame()]
[assembly: MelonAdditionalDependencies("BMXStreetsUI")]
namespace BMXStreetsUIExampleMod
{
    public class ExampleUIMelon : MelonMod
    {
        GameObject? mainMenu;
        GameObject? quickMenu;
        public override void OnLateInitializeMelon()
        {
            StreetsUI.RegisterForUICreation(OnUIReady);
        }

        // triggered by the API when the scene UI is loaded and ready
        void OnUIReady()
        {
            AutoModMenuPanel();
        }
        // setting up a simple panel with tab in the mod menu
        void AutoModMenuPanel()
        {
            var groups = new List<OptionGroup>();
            
            for (int i = 0; i < 5; i++) 
            {
                var mygroup = new OptionGroup($"Panel {i}");

                var myslider = new Slider("MySlider", 0, 50);
                myslider.SetCallBack(OnChangeValueFloat);
                myslider.decimalPlaces = 3;

                var myButton = new Button("Mybutton", "My Buttons Description");
                myButton.SetCallBack(OnClick);

                var myToggle = new Toggle("MyToggle");
                myToggle.SetCallBack(OnChangeValueBool);

                var mysteppedInt = new SteppedInt("MySteppedInt");
                mysteppedInt.choices.Add("Choice one");
                mysteppedInt.choices.Add("Choice two");
                mysteppedInt.choices.Add("Choice three");
                mysteppedInt.SetCallBack(OnChangeValueInt);

                mygroup.options.Add(myslider);
                mygroup.options.Add(myButton);
                mygroup.options.Add(myToggle);
                mygroup.options.Add(mysteppedInt);

                groups.Add(mygroup);
            }
            var myModMenu = new MenuPanel("MyMod", groups);

            var myOptionalPalette = new MenuPalette();
            myOptionalPalette.PanelOne = Color.green;
            myOptionalPalette.PanelTwo = Color.grey;
            myModMenu.palette = myOptionalPalette;

            mainMenu = StreetsUI.CreatePanel(myModMenu); // auto setup to modmenu by default. pass in a AutoTabSetup enum to customize
            LoggerInstance.Msg("AutosetupModmenuTab Complete");

            var myQuickMenu = new MenuPanel("MyMod", groups);
            quickMenu = StreetsUI.CreatePanel(myQuickMenu, StreetsUI.AutoTabSetup.ToQuickAccess, true, true, myModMenu); // passing in our mod menu will cause the returned panel to be setup with the exact same data, causing the panels to be synced.
                                                                                                                                      // myModMenu must have its panel setup first or otherwise have its listSet property set on it's UIPanel component.
            var quickButton = StreetsUI.CreateQuickMenuButton();
            StreetsUI.LinkQuickButtonToUIPanel(quickButton, quickMenu);
            StreetsUI.NewNotification("Example mod", "Example Inital setup notification");
        }

        /// Different types of callbacks used by sliders,toggles etc
        void OnChangeValueFloat(float value)
        {
            if (!AMenuIsOpen()) return;
            LoggerInstance.Msg($"MyFloatCallBack : {value}");
        }
        void OnChangeValueBool(bool value)
        {
            if (!AMenuIsOpen()) return;
            LoggerInstance.Msg($"MyBoolCallBack : {value}");
        }
        void OnChangeValueInt(int value)
        {
            if (!AMenuIsOpen()) return;
            LoggerInstance.Msg($"MyIntCallBack : {value}");
        }
        public void OnClick()
        {
            if (!AMenuIsOpen()) return;
            LoggerInstance.Msg("My Callback");
            StreetsUI.NewNotification("Example Mod", "OnButton Clicked Notification");
        }
        bool AMenuIsOpen()
        {
            if(mainMenu!= null)
            {
                if (mainMenu.activeInHierarchy) return true;
            }
            if (quickMenu != null)
            {
                if (quickMenu.activeInHierarchy) return true;
            }
            return false;
        }
    }
}
