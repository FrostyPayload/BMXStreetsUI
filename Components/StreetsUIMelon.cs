using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(BmxStreetsUI.Components.StreetsUIMelon), "BMX Streets UI", "version", "Author Name")]
[assembly: MelonGame()]

namespace BmxStreetsUI.Components
{
    public static class BuildInfo
    {
        public const string Name = "TestMod"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Mod for Testing"; // Description for the Mod.  (Set as null if none)
        public const string Author = "TestAuthor"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class StreetsUIMelon : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Log.Msg("BMX Manager OnApplicationStart");
        }

        public override void OnLateInitializeMelon() // Runs after OnApplicationStart.
        {
            Log.Msg("OnApplicationLateStart");
        }

        public override void OnSceneWasLoaded(int buildindex, string sceneName) // Runs when a Scene has Loaded and is passed the Scene's Build Index and Name.
        {
        }

        public override void OnSceneWasInitialized(int buildindex, string sceneName) // Runs when a Scene has Initialized and is passed the Scene's Build Index and Name.
        {
            Log.Msg("OnSceneWasInitialized: " + buildindex.ToString() + " | " + sceneName);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnSceneWasUnloaded: " + buildIndex.ToString() + " | " + sceneName);
        }

        public override void OnUpdate() // Runs once per frame.
        {
            // duplicates the system tab and system settings panel and manipulates to make a new menu
            // without losing all of the connections and events contained within ther objects
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // need to hook in when menu is open to find the objects i want to duplicate,
                // locate an object we can rely on in an onsceneload instead, "PauseMenu"?
                // Setup();
                MockAPICall();
            }
        }
        void MockAPICall()
        {
            var groups = new List<CustomMenuOptionGroup>();

            var mygroup = new CustomMenuOptionGroup("My menu");

            var myslider = new Slider("MySlider",0,50);
            myslider.SetCallBack(MyFloatCallback);

            var myButton = new Button("Mybutton","My Buttons Description");
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
            Log.Msg($"MyFloatCallBack : {value}");
        }
        void MyBoolCallback(bool value)
        {
            Log.Msg($"MyBoolCallBack : {value}");
        }
        void MyIntCallback(int value)
        {
            Log.Msg($"MyIntCallBack : {value}");
        }
        public void MyCallback()
        {
            Log.Msg("My Callback");   
        }
       
        public override void OnApplicationQuit() // Runs when the Game is told to Close.
        {
            MelonLogger.Msg("OnApplicationQuit");
        }

        public override void OnPreferencesSaved() // Runs when Melon Preferences get saved.
        {
            MelonLogger.Msg("OnPreferencesSaved");
        }

        public override void OnPreferencesLoaded() // Runs when Melon Preferences get loaded.
        {
            MelonLogger.Msg("OnPreferencesLoaded");
        }
    }

}
