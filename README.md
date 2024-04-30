# BMX Streets UI Mod

## Overview

TODO

### SaveLoad
Provide saveload system, either using in game system or separate

### UIHorizontalSelector
Currently manually handing the selector our refList, without it, the original SystemTab Settings show up as tab selections
Not showing an issue but implies missing link



## Types

### SmartData:
There's various types of `SmartData` such as `SmartData_Float`, `SmartData_String` etc and they represent one piece of data.
`SmartData` is abstract and so is `SmartData<Datatype>`, A `SmartData_Float` (such as FOV,Max lean angle etc) inherits `SmartData<smartDataFloatStuct>` which inherits `SmartData`. - stuct not struct.
`SmartData` hold Events which fire on change of values.

### SmartDataContainer:
The Object represented by the `.container` files found in `locallow`.
Hold a list of [SmartData](#smartdata), this list makes its way to being a set of options in the UI such as the audio option set.
if you create a container, name it and call save, it'll end up in `locallow/345345/` (steamid).
you can customise the `dataSaveExtension` from `.container`, mash uses `.setting` for some things.
if you populate the dataIdentifiers with a `CategoryListScriptableObject`, you can customise the path to have custom folders. - Category with an e.
provides functionality to its host [SmartDataContainerReferenceList](#smartdatacontainerreferencelist).

### SmartDataContainerReferenceList:
Holds a container.
Provides functoinality to its host [smartDataContainerReferenceListSet](#smartdatacontainerreferencelistset).
Its Save and load functions call down to its container.

### SmartDataContainerReferenceListSet:
Simply holds a name and a `list<SmartDataCntainerReferenceList>`.
the name is the title of the tab, and each entry in the list is a set of options in the menu e.g gameplay, audio, video settings list.

### SmartUIPalleteData:
Holds references to prefabs for the UI such as slider, button.
Holds colours and fonts.
Holds OnChange callbacks.
Handled by the [DataConfigPanel](#dataconfigpanel) and `SmartUI` system.

### Main menu tab:
Contains an `EventTrigger` component which hosts the events for highlight, unhighlight etc as well as the openMenu command of a specific `MGMenu`.
LocalizeString event will remove changes unless configured or turned off.

### DataConfigPanel:
Found on the Root of the Panel.
Master location of the [smartDataContainerReferenceListSet](#smartdatacontainerreferencelistset), in other words, all the data required for the panel.
Manages the creation of eveything below the panels `Content` Gameobject
