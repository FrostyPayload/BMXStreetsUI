# BMX Streets™ UI Mod

BMXStreetsUI is a MelonLoader mod which works as an API for other mods that want to easily integrate with the in-game UI system.

## Features:

- `Toggle`,`Slider`,`Button` and `SteppedInt` Element types
- Menu Bars
- Native Saving/Loading to `.container` files
- Native Timed Notifications
- Main menu and quick menu support with linkable data
- In-Built Character and Mod Menu's for simple setup options
<br></br>

## API Objects

### MenuPanel
A `MenuPanel` is the object you pass to `BMXStreetsUI.CreatePanel()` to have a panel created. A title and list of `OptionGroup` is required to create one.
The Title will be the text on your Tab if automatic tab setup is used, aswell as being used in the names of the various objects it creates to store it's data.

### OptionGroup
An OptionGroup simply houses a title and a list of `OptionBase` that go with it. In game, this title will appear above the selector icons when this list is selected, and the options inside
the group will be displayed below.

### OptionBase
Abstract class. All UI Elements inherit `OptionBase` and therefore go into an `OptionGroup`'s list. each supported UI Element customizes the `OptionBase` member's and callbacks for their use case.
For instance, a `Toggle` has it's own `Action<Bool>` callback that you can register to. Underneath, it receives the object passed to it by the `OptionBase` callback and refers that to your custom
callback, instead passing in the logic `value != 0`.

All UI elements have a title, description and defaultValue argument in their constructor.
- Title: The text next to your option's value.
- Description : The presence of data in this string will cause the info panel to show this string when the option is highlighted.
- DefaultValue : The initial value set in the underlying `SmartData`
- DataUnit : Optionally set the text displayed next to your value as it's unit of measure.
<br></br>
## Register for Creation of UI:
```
 public override void OnLateInitializeMelon()
 {
     StreetsUI.RegisterForUICreation(OnUIReady);
 }

 // triggered by the API when the scene UI is loaded and ready
 void OnUIReady()
 {
     AutoModMenuPanel();
 }
```
The Main UI only spawns in when the main world loads in. To simplify this, mods can use the StreetsUI.RegisterForUICreation method to register a void callback. This not only let's your mod execute as soon as the UI becomes available
but also put's your callback into a persistant List held by the BMXStreetsUI. If the user goes to the title screen, the world is unloaded and the UI modifications go with it. When the world is reloaded the registered callbacks will run again, recreating
all UI mods. This is worth keeping in mind when designing how your mod set's itself up.

<br></br>

## Quick Setup

```
 void AutoModMenuPanel()
 {
     var groups = new List<OptionGroup>();
     var mygroup = new OptionGroup($"My option set");

     var myslider = new Slider("MySlider", 0, 50);
     myslider.SetCallBack(OnChangeValueFloat);

     var myButton = new Button("Mybutton", "This button does stuff");
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
     
     var myModMenu = new MenuPanel("MyMod", groups);

     mainMenu = StreetsUI.CreatePanel(myModMenu); // auto setup to modmenu by default. pass in an AutoSetupOption enum to customize
     LoggerInstance.Msg("AutoSetupModMenuTab Complete");
 }
```
<br></br>

## Quick Menu
```
 var myQuickMenu = new MenuPanel("MyMod", groups);
 quickMenu = StreetsUI.CreatePanel(myQuickMenu, StreetsUI.AutoSetupOption.ToQuickAccess, true, true, myModMenu);
                                                                                                                          
 var quickButton = StreetsUI.CreateQuickMenuButton();
 StreetsUI.LinkQuickButtonToUIPanel(quickButton, quickMenu);
```
Setting up a QuickMenu consists of:
- Passing the `ToQuickAccess` enum to CreatePanel()
- Creating a Quick menu button
- Run LinkQuickButtonToUIPanel
  
`CreateQuickMenuButton()` has arguments for the `name` to give to the object and `spriteName` for the name of the sprite to attempt to find and load in. The returned object is
the new button's GameObject in the quickmenu heirarchy. This GameObject has a UnityEngine UI Image in it's children which hosts the Sprite.
<br></br>
Note the <b>original</b> menu being passed in to the creation of the quick menu. The presence of a created MenuPanel in this argument will cause the menu being created to use the same data in it's menu.
the created panel
<br></br>
<br></br>
## Notifications
```StreetsUI.NewNotification("Example mod", "Example Inital setup notification");```
This function will call anytime after UI creation. Specify a title, message and optional timeAlive float.

<br></br>
<br></br>

## Game Object Type info

### SmartData:
There's various types of `SmartData` such as `SmartData_Float`, `SmartData_String` etc and they represent one piece of data.
`SmartData` is abstract and so is `SmartData<Datatype>`, A `SmartData_Float` (such as FOV,Max lean angle etc) inherits `SmartData<smartDataFloatStuct>` which inherits `SmartData`. - stuct not struct.
`SmartData` hold Events which fire on change of values.
<br></br>
### SmartDataContainer:
The Object represented by the `.container` files found in `locallow`.
Hold a list of [SmartData](#smartdata), this list makes its way to being a set of options in the UI such as the audio option set.
if you create a container, name it and call save, it'll end up in `locallow/345345/` (steamid).
you can customise the `dataSaveExtension` from `.container`, mash uses `.setting` for some things.
if you populate the dataIdentifiers with a `CategoryListScriptableObject`, you can customise the path to have custom folders. - Category with an e.
provides functionality to its host [SmartDataContainerReferenceList](#smartdatacontainerreferencelist).
<br></br>
### SmartDataContainerReferenceList:
Holds a container.
Provides functoinality to its host [smartDataContainerReferenceListSet](#smartdatacontainerreferencelistset).
Its Save and load functions call down to its container.
<br></br>
### SmartDataContainerReferenceListSet:
Simply holds a name and a `list<SmartDataCntainerReferenceList>`.
the name is the title of the tab, and each entry in the list is a set of options in the menu e.g gameplay, audio, video settings list.
<br></br>
### SmartUIPalleteData:
Holds references to prefabs for the UI such as slider, button.
Holds colours and fonts.
Holds OnChange callbacks.
Handled by the [DataConfigPanel](#dataconfigpanel) and `SmartUI` system.
<br></br>
### Menu tab GameObjects:
Contain an `EventTrigger` component which hosts the events for highlight, unhighlight etc as well as the openMenu command of a specific `MGMenu`.
LocalizeString event will remove changes unless configured or turned off.
<br></br>
### MGMenu
The component that co-ordinates the menu's operation in conjunction with it's `MGMenuSystem` somewhere above it and `MGMenuSystemManager` somewhere above that.
- OpenMenu() is how the menu should open
- OpenLastMenu() is how it should close
- OnEnter and OnEnterClose events function as implied
<br></br>
### DataConfigPanel:
Found on the Root of the Panel.
Master location of the [smartDataContainerReferenceListSet](#smartdatacontainerreferencelistset), in other words, all the data required for the panel.
Manages the creation of eveything below the panels `Content` Gameobject
