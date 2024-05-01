using MelonLoader;
using Il2Cpp;
using BmxStreetsUI;
using UnityEngine;
using UnityEngine.EventSystems;

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
             SetupMainMenuPanel();
            //SetupMenuWithCustomEntryExit();
        }

        public override void OnUpdate()
        {
            if(Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SetupMenuWithCustomEntryExit();
            }
        }
        void SetupMainMenuPanel()
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
        void SetupMenuWithCustomEntryExit()
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

            if (!API.CreatePanel(mymenu))
            {
                LoggerInstance.Msg($"Failed to create panel");
                return;
            }
            if (mymenu.panel == null)
            {
                LoggerInstance.Msg($"Mymenu panel is null");
                return;
            }

            // the UIPanel should now exist in you CustomMenu.Panel field
            foreach (var config in GameObject.FindObjectsOfType<DataConfigPanel>(true).Where(dataconfig => dataconfig.gameObject.name == "BMXS_Character Selection Menu")) 
            {
                foreach(var trigger in config.GetComponentsInChildren<EventTrigger>())
                {
                    var open = new EventTrigger.Entry();
                    open.eventID = EventTriggerType.Submit;
                    open.callback.AddListener(new System.Action<BaseEventData>((data) => { mymenu.panel.OnOpen(); }));
                    trigger.triggers.Add(open);
                    trigger.delegates.Add(open);
                }
               // config.OnChangedSelection.AddListener(new System.Action(() => { LoggerInstance.Msg($"Character Tab "); MyCustomCallBack(config.generalDataSet.currentSelectedIndex); }));
               // config.GetSelectedDataScriptableObject();
                mymenu.panel.transform.SetParent(config.transform.parent, false);
                mymenu.panel.Init();
                mymenu.panel.RunSetup();

                break;
                
            }
            return;

            
        }
        void MyCustomCallBack(int characterIndex)
        {
            LoggerInstance.Msg($"custom callback received {characterIndex}");
            
        }
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
