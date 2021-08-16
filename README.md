# MotionBlurDisable

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that disables all motion blur. This makes the desktop experience *much* more usable.

This will no longer be needed once https://github.com/Neos-Metaverse/NeosPublic/issues/322 is resolved.

## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place [MotionBlurDisable.dll](https://github.com/zkxs/MotionBlurDisable/releases/latest/download/MotionBlurDisable.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.

## FAQ
### What does this actually do?
It force disables the Unity MotionBlur post processing effect for all cameras by replacing the camera setup method with a slightly modified version.

### Is this against guidelines?
Maybe. [Read more](https://github.com/zkxs/NeosModLoader/blob/master/doc/neos_guidelines.md).
