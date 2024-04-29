using MelonLoader;
using Il2Cpp;
using BmxStreetsUI;
using UnityEngine;

[assembly: MelonInfo(typeof(BMXStreetsUIExampleMod.ExampleUIMelon), "BMX Streets UI Example mod", "version", "Author Name")]
[assembly: MelonGame()]
namespace BMXStreetsUIExampleMod
{
    public static class BuildInfo
    {
        public const string Name = "BMXStreetsUIExampleMod"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Mod for testing and understanding the BMXStreetsUI API"; // Description for the Mod.  (Set as null if none)
        public const string Author = "FrostyP/LineRyder"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }
    public class ExampleUIMelon : MelonMod
    {
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
        }
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SetupMyUI();
            }
        }
        void SetupMyUI()
        {
            var groups = new List<CustomMenuOptionGroup>();

            var mygroup = new CustomMenuOptionGroup("My menu");

            var myslider = new Slider("MySlider", 0, 50);
            myslider.SetCallBack(MyFloatCallback);

            var myButton = new Button("Mybutton", "My Buttons Description");
            myButton.SetCallBack(MyCallback);

            var myToggle = new Toggle("MyToggle");
            myToggle.SetCallBack(MyBoolCallback);

            var mysteppedInt = new SteppedInt("MySteppedInt");
            mysteppedInt.choices.Add("Choice one");
            mysteppedInt.choices.Add("Choice two");
            mysteppedInt.choices.Add("Choice three");
            mysteppedInt.SetCallBack(MyIntCallback);

            mygroup.options.Add(myslider);
            mygroup.options.Add(myButton);
            mygroup.options.Add(myToggle);
            mygroup.options.Add(mysteppedInt);

            groups.Add(mygroup);

            var mymenu = new CustomMenu("MyTab", groups);
            API.AddMenu(mymenu);
        }
        void MyFloatCallback(float value)
        {
            LoggerInstance.Msg($"MyFloatCallBack : {value}");
        }
        void MyBoolCallback(bool value)
        {
            LoggerInstance.Msg($"MyBoolCallBack : {value}");
        }
        void MyIntCallback(int value)
        {
            LoggerInstance.Msg($"MyIntCallBack : {value}");
        }
        public void MyCallback()
        {
            LoggerInstance.Msg("My Callback");
        }
    }
}
