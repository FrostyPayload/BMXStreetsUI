using Il2Cpp;
using Il2CppMG_UI.MenuSytem;
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
            var modBar = API.CreateMenuBar();
            var ModMenuTab = API.CreateTab("Mods",API.settingsTab.transform.parent);
            if (modBar == null || ModMenuTab == null)
            {
                Log.Msg($"Error setting up ModMenu", true);
                return;
            }
            modBar.name = "StreetsUIModsMenu";
            var mg = modBar.GetComponent<MGMenu>();
            API.LinkTabClickToAction(ModMenuTab, new Action(() => { modBar.SetActive(true); mg.OpenMenu(); }));
            ModMenu.modBar = modBar;
        }
        public static void CharacterMenuSetup()
        {
            CharacterBar = API.CreateMenuBar();
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

            var characterTab = GameObject.FindObjectsOfType<EventTrigger>( true ).Where((EventTrigger trig) => { return trig.gameObject.name.Contains("CHARACTER"); }).First();
            API.LinkTabClickToAction(characterTab.gameObject, new System.Action(() => 
            { 
                foreach (var configPanel in characterTab.GetComponentsInChildren<DataConfigPanel>(true).Where(dataconfig => dataconfig.gameObject.name.Contains(Constants.CharacterSelectionPanel)))
                {
                    //CharacterBar.gameObject.transform.SetParent(configPanel.transform.parent, false);
                    var menu = configPanel.gameObject.GetComponent<MGMenu>();
                    menu.OnEnterOpen = new UnityEngine.Events.UnityEvent();
                    menu.OnEnterClose = new UnityEngine.Events.UnityEvent();
                    menu.OnEnterOpen.RemoveListener(new System.Action(() => SetCharacterButtonTriggers(configPanel.gameObject, mg)));
                    menu.OnEnterOpen.AddListener(new System.Action(() => SetCharacterButtonTriggers(configPanel.gameObject, mg)));
                    SetCharacterButtonTriggers(configPanel.gameObject, mg);
                
                    configPanel.Start();
                    configPanel.Validate();
                    if(menu.OnOpenRaiseEvents!= null) menu.OnOpenRaiseEvents.Clear();

                    break;
                }
                mg.Awake();
                if(mg.GetMenuSystem() != null)
                {
                    mg.GetMenuSystem().Init();
                }

            }),false);

        }
        static void SetCharacterButtonTriggers(GameObject TriggerParent,MGMenu mg)
        {
            foreach (var trigger in TriggerParent.GetComponentsInChildren<EventTrigger>(true))
            {
                API.LinkTabClickToAction(trigger.gameObject, new System.Action(() => { mg.gameObject.SetActive(true); mg.OpenMenu(); }));
                Log.Msg($"Added trigger for {trigger.gameObject.name}");
            }
        }
        public static void AddToCharacterMenu(MenuPanel menu)
        {
            // auto align ui depending on existing children, setup gridLayout?
            var tab = API.CreateTab(menu.TabTitle, CharacterBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
            API.LinkTabClickToAction(tab, menu.panel.OnOpen);
        }
        public static void AddToModMenu(MenuPanel newMenu)
        {
            var tab = API.CreateTab(newMenu.TabTitle, modBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform);
            
            API.LinkTabClickToAction(tab, newMenu.panel.OnOpen);
        }

        public static Transform GetModMenuTabParent()
        {
            return modBar.GetComponentInChildren<MenuTabGroup>(true).gameObject.transform;
        }
    }
}
