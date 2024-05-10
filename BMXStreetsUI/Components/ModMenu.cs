﻿using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BmxStreetsUI.Components
{
    internal class ModMenu
    {
        static GameObject modBar,CharacterBar;
        static int ModMenus { get { return modBar != null ? modBar.GetComponentsInChildren<MGMenu>().Count : 0; } }
        static int CharacterMenus { get { return CharacterBar != null ? CharacterBar.GetComponentsInChildren<MGMenu>().Count : 0; } }
        public static void ModMenuSetup(GameObject settingsTab)
        {
            var modBar = StreetsUI.CreateMenuBar();
            var ModMenuTab = StreetsUI.CreateTab("Mods",StreetsUI.settingsTab.transform.parent);
            if (modBar == null || ModMenuTab == null)
            {
                Log.Msg($"Error setting up ModMenu", true);
                return;
            }
            modBar.name = "StreetsUIModsMenu";
            var mg = modBar.GetComponent<MGMenu>();
            StreetsUI.LinkTabTriggerToAction(ModMenuTab, new Action(() => { modBar.SetActive(true); mg.OpenMenu(); }));
            ModMenu.modBar = modBar;
        }
        public static void CharacterMenuSetup()
        {
            CharacterBar = StreetsUI.CreateMenuBar();
            if (CharacterBar == null)
            {
                Log.Msg($"Error setting up Character Menu", true);
                return;
            }
            CharacterBar.name = "StreetsUICharacterMenu";
            var mg = CharacterBar.GetComponent<MGMenu>();
            mg.linkedPreviousMenu = null;
            mg.OnOpenRaiseEvents = new Il2CppSystem.Collections.Generic.List<GameEvent>();
            mg.OnEnterOpen = new UnityEngine.Events.UnityEvent();
            mg.OnEnterClose = new UnityEngine.Events.UnityEvent();
            mg.OnCloseRaiseEvents = new Il2CppSystem.Collections.Generic.List<GameEvent>();
            mg.currentMenuSelectable = null;
            mg.isFirstMenu = false;
            mg.enabled = true;
            mg.Init();
            if (mg.GetMenuSystem() != null)
            {
                mg.GetMenuSystem().Init();
            }
            MelonCoroutines.Start(AwaitCharacterSetup());
        }
        static System.Collections.IEnumerator AwaitCharacterSetup()
        {
            while(GameObject.FindObjectOfType<CharacterSelectionManager>() == null)
            {
                yield return new WaitForSeconds(1);
            }
            
            OnCharacterUISetup();
        }
        static void OnCharacterUISetup()
        {
            var characterTab = GameObject.FindObjectsOfType<EventTrigger>(true).Where((EventTrigger trig) => { return trig.gameObject.name.Contains("CHARACTER"); }).First();
            var mg = CharacterBar.GetComponent<MGMenu>();
            foreach (var configPanel in GameObject.FindObjectsOfType<DataConfigPanel>(true).Where(dataconfig => dataconfig.gameObject.name.Contains(Constants.CharacterSelectionPanel)))
            {
                var characterMgMenu = configPanel.gameObject.GetComponent<MGMenu>();
                characterMgMenu.currentMenuSelectable = null;
                characterMgMenu.OnEnable();
                characterMgMenu.OpenMenu();
                configPanel.Start();
                configPanel.Init();
                configPanel.PopulateList();
                characterMgMenu.OpenLastMenu();
                characterMgMenu.OnDisable();
                characterMgMenu.OnEnterOpen.RemoveListener(new System.Action(() => SetCharacterButtonTriggers(configPanel.gameObject, mg)));
                characterMgMenu.OnEnterOpen.AddListener(new System.Action(() => { SetCharacterButtonTriggers(configPanel.gameObject, mg); }));
                characterMgMenu.Init();
                SetCharacterButtonTriggers(configPanel.gameObject, mg);

                break;
            }
        }
        static void SetCharacterButtonTriggers(GameObject TriggerParent,MGMenu mg)
        {
            foreach (var trigger in TriggerParent.GetComponentsInChildren<EventTrigger>(true))
            {
                StreetsUI.LinkTabTriggerToAction(trigger.gameObject, new System.Action(() => { mg.gameObject.SetActive(true); mg.OpenMenu(); }),false);
            }
        }
        public static void AddToCharacterMenu(MenuPanel menu)
        {
            // auto align ui depending on existing children, setup gridLayout?
            var tab = StreetsUI.CreateTab(menu.TabTitle, CharacterBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
            StreetsUI.LinkTabTriggerToAction(tab, menu.panel.OnOpen);
        }
        public static void AddToModMenu(MenuPanel newMenu)
        {
            var tab = StreetsUI.CreateTab(newMenu.TabTitle, modBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
            StreetsUI.LinkTabTriggerToAction(tab, newMenu.panel.OnOpen);
        }

        public static Transform GetModMenuTabParent()
        {
            return modBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform;
        }
    }
}
