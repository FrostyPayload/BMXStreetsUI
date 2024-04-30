using MelonLoader;
using Il2Cpp;
using BmxStreetsUI;
using UnityEngine;

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
            SetupMyUI();
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
        }
        public override void OnUpdate()
        {
            
        }
        void SetupMyUI()
        {
            var groups = new List<CustomMenuOptionGroup>();

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

            var mymenu = new CustomMenu("MyTab", groups);

            var myOptionalPallete = new CustomMenuPallete();
            myOptionalPallete.PanelOne = Color.green;
            myOptionalPallete.PanelTwo = Color.grey;
            mymenu.pallete = myOptionalPallete;

            API.AddMenu(mymenu);
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
