using MelonLoader;
using Il2Cpp;
using BmxStreetsUI;
using UnityEngine;
using UnityEngine.EventSystems;
using Il2CppMG_UI.MenuSytem;

[assembly: MelonInfo(typeof(BMXStreetsUIExampleMod.ExampleUIMelon),"BMXStreetsUIExampleMod", "1.0.0", "FrostyP/LineRyder")]
[assembly: MelonGame()]
[assembly: MelonAdditionalDependencies("BMXStreetsUI")]
namespace BMXStreetsUIExampleMod
{
    public class ExampleUIMelon : MelonMod
    {
        /// <summary>
        /// The API will store UI requests that come in before the MainMenu exists and build them as the main level loads in
        /// </summary>
        public override void OnLateInitializeMelon()
        {
            AutoSetupModMenuTab();
            SetupMenuAttachedToExistingButton();
        }

        public override void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SetupMenuAttachedToExistingButton();
            }
        }
        void AutoSetupModMenuTab()
        {
            var groups = new List<CustomMenuOptionGroup>();
            
            for (int i = 0; i < 5; i++) 
            {
                var mygroup = new CustomMenuOptionGroup("My menu");

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

            var mymenu = new CustomMenu("MyTab", groups);

            var myOptionalPallete = new CustomMenuPallete();
            myOptionalPallete.PanelOne = Color.green;
            myOptionalPallete.PanelTwo = Color.grey;
            mymenu.pallete = myOptionalPallete;

            API.CreatePanel(mymenu);
        }
        void SetupMenuAttachedToExistingButton()
        {
            // create menu in the same way as an autosetup, but pass in the custom enum to stop tab creation and autosetup
            var groups = new List<CustomMenuOptionGroup>();

            for (int i = 0; i < 5; i++)
            {
                var mygroup = new CustomMenuOptionGroup("My menu");

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

            var mymenu = new CustomMenu("MyCustomMenu", groups);

            var myOptionalPallete = new CustomMenuPallete();
            myOptionalPallete.PanelOne = Color.green;
            myOptionalPallete.PanelTwo = Color.grey;
            mymenu.pallete = myOptionalPallete;

            mymenu.OnMenuCreation = OnMenuCreate; // we want to work on the UI after it's created, but we're calling to the API way before the UI exists.
            if (!API.CreatePanel(mymenu, API.AutoTabSetup.Custom))
            {
                LoggerInstance.Msg($"Failed to create panel");
                return;
            }
            
        }
        void OnMenuCreate(CustomMenu mymenu)
        {
            LoggerInstance.Msg("MyCustomMenu Created, setting up..");

            MelonCoroutines.Start(WaitForCharacterTabToSetup(mymenu));
        }
        System.Collections.IEnumerator WaitForCharacterTabToSetup(CustomMenu mymenu)
        {
            yield return new WaitForSeconds(5);

            // the UIPanel should now exist in you CustomMenu.Panel field, now find the button(s) that we want to use an attach the panels onOpen to the buttons EventTrigger
            foreach (var config in GameObject.FindObjectsOfType<DataConfigPanel>(true).Where(dataconfig => dataconfig.gameObject.name.Contains("BMXS_Character Selection Menu")))
            {
                foreach (var trigger in config.gameObject.GetComponentsInChildren<EventTrigger>(true))
                {
                    LoggerInstance.Msg($"{trigger.ToString()}");
                    //API.LinkTabClickToAction(trigger.gameObject, mymenu.panel.OnOpen);
                    var open = new EventTrigger.Entry();
                    open.eventID = EventTriggerType.Submit;
                    open.callback.AddListener(new System.Action<BaseEventData>((data) => { mymenu.panel.gameObject.SetActive(true); mymenu.panel.OnOpen(); }));
                    trigger.triggers.Add(open);
                    trigger.delegates.Add(open);
                    
                    LoggerInstance.Msg($"Added trigger for {trigger.gameObject.name}");
                }

                mymenu.panel.transform.SetParent(config.transform.parent, false);
                mymenu.panel.Init();
                mymenu.panel.RunSetup();
                break;
            }
        }

        void MyCustomCallBack(int characterIndex)
        {
            LoggerInstance.Msg($"custom callback received {characterIndex}");
            
        }

        /// Different types of callbacks used by sliders,toggles etc
        void OnChangeValueFloat(float value)
        {
            LoggerInstance.Msg($"MyFloatCallBack : {value}");
        }
        void OnChangeValueBool(bool value)
        {
            LoggerInstance.Msg($"MyBoolCallBack : {value}");
        }
        void OnChangeValueInt(int value)
        {
            LoggerInstance.Msg($"MyIntCallBack : {value}");
        }
        public void OnClick()
        {
            LoggerInstance.Msg("My Callback");
        }
    }
}
