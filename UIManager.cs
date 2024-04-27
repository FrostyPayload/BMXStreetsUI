using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using Il2Cpp;
using UnityEngine;
using UnityEngine.Events;
using Il2CppMG_Gameplay.UI;
using Il2CppMG_UI.MenuSytem;
using Il2CppMG_UI;
using Il2CppMichsky.UI.ModernUIPack;
using Il2CppTMPro;
using UnityEngine.EventSystems;
using System.Reflection;
using Il2CppMG_PlaybackSystem;
using Il2CppSystem.Reflection;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization;
using Il2CppMG_Core.MG_SmartData.SaveLoad;

[assembly: MelonInfo(typeof(BmxStreetsUI.UIManager), "BMX Streets UI", "version", "Author Name")]
[assembly: MelonGame()]

namespace BmxStreetsUI
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

    public class UIManager : MelonMod
    {
        #region GUIConcept
        bool on;
        BMXFreeformPoseData[] poses;
        BMXFreeformPoseData smith;
        Vector3 smithRoot;
        float rootX = 0f;
        float rootY = 0f;
        #endregion


        UIPanel GrindTabTest;
        GrindPosePanel GrindsPanel;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("BMX Manager OnApplicationStart");
        }

        public override void OnLateInitializeMelon() // Runs after OnApplicationStart.
        {
            MelonLogger.Msg("OnApplicationLateStart");
        }

        public override void OnSceneWasLoaded(int buildindex, string sceneName) // Runs when a Scene has Loaded and is passed the Scene's Build Index and Name.
        {


        }

        public override void OnSceneWasInitialized(int buildindex, string sceneName) // Runs when a Scene has Initialized and is passed the Scene's Build Index and Name.
        {
            MelonLogger.Msg("OnSceneWasInitialized: " + buildindex.ToString() + " | " + sceneName);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnSceneWasUnloaded: " + buildIndex.ToString() + " | " + sceneName);
        }

        public override void OnUpdate() // Runs once per frame.
        {
            // the OnGUI concept
            if (Input.GetKeyDown(KeyCode.F))
            {
                on = !on;
                Cursor.lockState = on ? CursorLockMode.Confined : CursorLockMode.Locked;
                Cursor.visible = on;
            }

            // duplicates the system tab and system settings panel and manipulates to make a new menu
            // without losing all of the connections and events contained within ther objects
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // need to hook in when menu is open to find the objects i want to duplicate,
                // locate an object we can rely on in an onsceneload instead, "PauseMenu"?
                Setup();
            }

        }
        void Setup()
        {
            GameObject systemSettingsPanel = GameObject.Find("System Tab Settings");
            var systemSettingsTab = GameObject.Find("SYSTEM-TAB");
            if(systemSettingsPanel == null) // does it need to be active for .Find()
            {
                 foreach (var menu in UnityEngine.Object.FindObjectsByType<MGMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                 {
                    if (menu.gameObject.name.ToLower().Contains("system tab settings"))
                    {
                        systemSettingsPanel = menu.gameObject;
                        break;
                    }
                 }
            }

            if (systemSettingsPanel != null && systemSettingsTab != null)
            {
                LoggerInstance.Msg($"Setting up new UI");
                var duplicateTab = UnityEngine.Object.Instantiate(systemSettingsTab);
                var duplicateSettingPanel = UnityEngine.Object.Instantiate(systemSettingsPanel);
                if (duplicateTab == null)
                {
                    LoggerInstance.Msg("Tab is null");
                    return;
                }
                if (duplicateSettingPanel == null)
                {
                    LoggerInstance.Msg("settings is null");
                    return;
                }
                GrindsPanel = new GrindPosePanel();
                GrindTabTest = duplicateSettingPanel.AddComponent<UIPanel>(); // cant inherit and use virtuals?
                GrindTabTest.tab = duplicateTab;
                GrindTabTest.tabParent = systemSettingsTab.transform.parent.gameObject;
                GrindTabTest.Init();
                GrindsPanel.SetupData(GrindTabTest);
                GrindTabTest.RunSetup();
                // perhaps want to prefab the completed setup and keep it aside

                LoggerInstance.Msg("Setup complete");
            }

        }

        public override void OnFixedUpdate() // Can run multiple times per frame. Mostly used for Physics.
        {
            //MelonLogger.Msg("OnFixedUpdate");
        }

        public override void OnLateUpdate() // Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        {
            //MelonLogger.Msg("OnLateUpdate");
        }

        public override void OnGUI() // Can run multiple times per frame. Mostly used for Unity's IMGUI.
        {
            // concept
            if (!on) return;
            if (poses == null | smith == null) return;

            GUILayout.BeginVertical(new GUILayoutOption[2] { GUILayout.Height(600), GUILayout.Width(200) });

            GUILayout.TextArea("Bars Rotation");
            smith._barsRot = GUILayout.HorizontalSlider(smith._barsRot, -90, 90);
            GUILayout.TextArea("Cranks Rotation");
            smith._cranksRot = GUILayout.HorizontalSlider(smith._cranksRot, -90, 90);
            GUILayout.TextArea("left pedal Rotation");
            smith._leftPedalRot = GUILayout.HorizontalSlider(smith._leftPedalRot, -90, 90);
            GUILayout.TextArea("right pedal Rotation");
            smith._rightPedalRot = GUILayout.HorizontalSlider(smith._rightPedalRot, -90, 90);

            GUILayout.TextArea("Postion X");
            rootX = GUILayout.HorizontalSlider(rootX, -1, 1);

            GUILayout.TextArea("Postion Z");
            rootY = GUILayout.HorizontalSlider(rootY, -1, 1);
            smith._rootPos = smithRoot + new Vector3(rootX, 0, rootY);

            GUILayout.TextArea("Gravity");
            Physics.gravity = new Vector3(0, GUILayout.HorizontalSlider(Physics.gravity.y, -1, -20), 0);

            GUILayout.EndVertical();
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
