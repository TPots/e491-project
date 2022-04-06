# Introduction

## Terminology

The following terms are used in the documentation. The terms definitions are as follows:

| Term  | Description |
| --- | --- |
| Root | The game object representing the physical device used to track a user's position. Acts as the origin (0,0,0) for the Unity scripts used for tracking and projection.  |
| Game object | A Unity C# class used to define objects that exist in the editor. |
| Scriptable object | A C# class which defines a configurable asset that can be instantiated by Unity. |
| Setup object | A scriptable object that defines the cave configuration parameters used to instantiate the game objects required by the cave scripts. |
|Prefab| A pre-configured Unity game object. |

## Coordinate System

In order to configure the setup, the following coordinate system is used across all scripts assuming an initial reference of a user standing facing a tracking device.

|Unity Axis Direction| Physical Equivalent |
|---|---|
|X+| Right|
|X-| Left|
|Y+| Up|
|Y-| Down |
|Z+|Forwards|
|Z-|Backwards|

# Setup

## Downloading from Github and Adding to a Unity Project

<img src="/pictures/github.png" width = "25%" height="25%">
Under the "code" menu on Github download the repository or otherwise clone into a Unity project folder. Open the project in Unity and wait for Unit to load in assets added as part of the repository.

## Template Configuration

The data structure defining the configuration template is encoded in a scriptable object which can be created by right clicking on the asset menu in Unity and selecting "Cave Setup" under the "create" sub-menu. Setup object's are spawned with the default name "new cave setup".

<img src="/pictures/asset_menu.png" width = "50%" height="50%">

The setup editor can open by either double clicking on the "new setup object" or by clicking the button "Open Editor" under the inspector for the setup object.

<img src="/pictures/inspector.png" width = "50%" height="50%">

After opening the editor, the following window will open which the setup object can be configured.

<img src="/pictures/template_pane.png" width = "50%" height="50%">

## Label

<img src="/pictures/label.png" width = "50%" height="50%">

The label field controls the name of the Unity game object spawned. Game objects spawned by the cave scripts are tagged with the "cave-<label>" prefix. Note that when spawning the game objects, the script will look for game objects with labels matching "cave-<label>" and delete them before spawning a new game object  

### Root

<img src="/pictures/root.png" width = "50%" height="50%">
 
 The root pane contains the fields used to define the coordinates and orientation of the root entity in the Unity world space.
 
| Field | Description |
| --- | --- |
| Position | Defines the XYZ coordinates of the root object in Unity world coordinates. Values are in units of meters. |
| Rotation | Defines the XYZ euler rotations of the root object in Unity world coordinates. Values are in units of degrees.|
| Root Scale | Scales the proportions of the root object in order scale the setup to the proportions of the Unity project being projected |
 
Note that the "Root Scale" parameter is used to define the scale of all objects in the setup object including the display sizes.
 
### User

<img src="/pictures/user.png" width = "50%" height="50%">
 
| Field | Description |
| --- | --- |
| Position | Defines the XYZ coordinates of the user object in Unity world coordinates relative to the root object. Values are in units of meters. |
| Rotation | Defines the XYZ euler rotations of the user object in Unity world coordinates relative to the root object. Values are in units of degrees. |
| Enable User Tracking| Enables external user tracking data to be written into the position field of the user object. If left false, the user position is defined by the position entered into the data structure. |

### Displays

#### Number of displays

<img src="/pictures/number_of_displays.png" width = "50%" height="50%">
 
The number of displays the setup contains are entered into the field "Number of Displays''. Note that Unity is limited to a maximum of eight displays so the field is capped at eight.

#### Display Configuration

<img src="/pictures/display.png" width = "50%" height="50%">
 
  As the number of displays changes, the left pane is dynamically updated with buttons for each display defined. Upon selecting a display from the left pane, the configurable parameters are drawn on the right pane.
 
| Field | Description |
| --- | --- |
| Tag | The name of the display. By default displays are tagged with a numeric index value. Tag is used to define the name of the display used in both the editor window and the name of the display game object spawned in unity. |  
| Position | Defines the XYZ coordinates of the display in Unity world coordinates relative to the root object. Values are in units of meters. |
| Rotation | Defines the XYZ euler rotations of the display object in Unity world coordinates relative to the root object. Values are in units of degrees. |
| Display Size| The display size of the currently selected display. The "X" field defines the width of the display. The "Y" field defines the height of the display. Values are in units of meters. |
  | Camera Draw Distance | Defines the draw distance of the Unity rendering camera attached to the display. Value is in units of meters. |
  | Enable Alignment Structures | Enables the alignment structure prefab objects in Unity attached to each of the camera game objects. |

### Function Buttons

<img src="/pictures/functions.png" width = "50%" height="50%">
 
  |Field|Description|
  |---|---|
  |Generate Game Object| Sanitizes the Unity project of Unity game objects with the name "cave-<label>" and generates the setup defined by the setup structure.  |
  |Randomize Setup| Generates and writes random values into the display position and rotation fields. Used to quickly generate a configuration where position and rotation is not important. |
 
# Aligning the Configuration with a Physical Setup
 
  To ensure the 3D space is projected correctly onto the displays it's critical that the display coordinates in Unity are equal to the physical display.  To accomplish this we recommend that for each of the displays in the configuration, the following steps are taken:
 
  1. "Enable Alignment Structures" toggled in the display configuration
  2. Stand a distance of one meter normal to the display face.
  3. Eye height equal to the display center.
  4. Change the position of the display in Unity until the alignment structure is correct (examples of alignment offsets are below).
  5. un-select "Enable Alignment Structures" in the display configuration.
 
  Once each of the displays have been calibrated, regenerate the setup using the "Generate Setup Objects" button in the "Cave Setup" editor window.
 
  ## Example of Correct Structure Alignment
 
  <img src="/pictures/alignment_correct.png" width = "50%" height="50%">
 
  ## Example of Left Misalignment
 
  <img src="/pictures/alignment_left.png" width = "50%" height="50%">

  ## Example of Right Misalignment

  <img src="/pictures/alignment_right.png" width = "50%" height="50%">

  ## Example of High Height Misalignment

  <img src="/pictures/alignment_high.png" width = "50%" height="50%">

  ## Example of Low Height Misalignment

  <img src="/pictures/alignment_low.png" width = "50%" height="50%">




