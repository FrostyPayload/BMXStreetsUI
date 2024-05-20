using MelonLoader;
using BmxStreetsUI;
using UnityEngine;

[assembly: MelonInfo(typeof(BMXStreetsUIExampleMod.ExampleUIMelon),"BMXStreetsUIExampleMod", "1.0.0", "FrostyP/LineRyder")]
[assembly: MelonGame()]
[assembly: MelonAdditionalDependencies("BMXStreetsUI")]
namespace BMXStreetsUIExampleMod
{
    public class ExampleUIMelon : MelonMod
    {
        GameObject? mainMenu;
        GameObject? quickMenu;
        GameObject? characterMenu;
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
            var myQuickMenu = new MenuPanel("MyMod", groups);
            var myCharacterMenu = new MenuPanel("MyMod", groups);
            mainMenu = StreetsUI.CreatePanel(myModMenu); // auto setup to modmenu by default. pass in a AutoSetupOption enum to customize

            quickMenu = StreetsUI.CreatePanel(myQuickMenu, StreetsUI.AutoSetupOption.ToQuickAccess, true, true,"Lightbulb", myModMenu); // passing in our mod menu will cause the returned panel to be setup with the exact same data, causing the panels to be synced.
                                                                                                                                        // myModMenu must have its panel setup first or otherwise have its listSet property set on it's UIPanel component.
            
            characterMenu = StreetsUI.CreatePanel(myCharacterMenu, StreetsUI.AutoSetupOption.ToCharacter, true, true, "", myModMenu); // The sprite string is only used for quick menu button's

            LoggerInstance.Msg("AutoSetupModMenuTab Complete");
            StreetsUI.NewNotification("Example mod", "Example inital setup notification");
        }

        
        /// Different types of callbacks used by sliders,toggles,steppedInt's and buttons
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

        // Once callbacks become linked to SmartData's OnValueChanged event, and load is called for instance, all SmartData OnValueChanged callbacks fire with the loaded values whether your receiving code is ready for that or not.
        // nice bike build you got there.
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
