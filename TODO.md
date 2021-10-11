# Features to implement

> Things in here will be implemented soon. (mostly.)

## Hooah!

- **Bug Fixes**
  - Improve performances 
  - [x] Fix HijackNeck and Eyes
 
- **Features**
  - [x] Add new adjustment field for the new "D" assets
  - [ ] Serializable Camera Tracking Folder

----

## Hooah Utility

### Next Release

- **Studio Reference Serialization**
  - [ ] Make Studio Reference Serialization
    - [x] Save
    - [ ] Import
- **QoL**
  - [ ] Copy the parameter of the item when copying the item.
    - Make sure importing the scene also works
  - [ ] Clamp the window location to not go  over the screen boundary to prevent bruh moment 

### Planned 
- **UI**
  - [ ] Add dependency between fields (like some menu should appear when certain option is true.)
- **Structure Improvement**
  - [ ] Extensible Modular Serialization Structure
    - Make other plugin developer to work on their own things with Hooah UI Utilities
  - [ ] Remove all cringe temporary codes
- **Asset Reference**
  - [ ] Reference External File or Asset and assign the asset for the target with the UI.
    - [ ] Create Texture Picker
    - [ ] create material picker
    - [ ] create asset picker 
      - for something like custom options.
- **Render Context**
  - [ ] Render Texture based on the json file
  - [ ] Set Mateiral based on the json file
    - [ ] Implementing the Material on studio item
- **Performance Improvement**
  - [ ] Cache the form and UI (queue?)
  - [ ] Create UI Material Hacks
    - [ ] Darkmode
    - [ ] Lightmode
    - [ ] Overall colorscheme
- **I18n Support**
  - [ ] Support Japanese
  - [ ] Support Korean
  - [ ] Support Chinese

----

## Hooah Smug Face

### 1.0.0 Release

- **Creating workflow to make new face shape and expression**
  - [ ] Blender Tutorial
  - [ ] Creating and packing the face tutorial
  - [ ] Modding tool integrating
   
- **Creating Runtime Plugin**
  - [x] Creating Face Offset Datafile
  - [x] Adding new face expression
  - [x] Adding new face shape
  - [ ] Adjusting face mesh permanently based on the mesh offset file.
    - [ ] Cache original mesh data to prevent unknown fuckups
    
- **Creating UI for New Face Expression**
  - *Character Editor*
    - [ ] Loading Face Offset and applying the mesh offset
    - [ ] Make sure Face offset will not interfere with other mods and calculations
      - Excluding Edge cases like Abnormal Face Offset Data  (like intentional abomination)
  - *Studio*
    - [ ] Simple Face Expression
    - [ ] Composed Face Expression (one slider, multiple adjustment)
    - [ ] Face Expression Preset

----
 
## Hooah Launch

- **Creating Launch UI**
- **Search Items and Actions**
  - [ ] Invoke the studio action immediately.
  - [ ] Load studio item immediately.

----

## Pending Ideas

- **Lite Scene**
  - a scene-like file that is focusing on loading things more faster.
  - focusing on importing things
    - adjust the center when save
    - import to the center of the camera
