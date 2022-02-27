# More Atmosphere Audio POC

## Install

Currently you need to either follow the build guide down below or use the R2ModMan. 

## Usage
By suppying a `.mp3`, `.aif`, `.wav` or a  `.ogg` file in the `CustomData\\Audio\\<Abmient | Music>` folder. The audio will be registered to the dropdown menu in the atmosphere settings. This is a POC which will most likely be taken over.

## How to Compile / Modify

Open ```.sln``` in Visual Studio.

You will need to add references to:

```
* BepInEx.dll  (Download from the BepInEx project.)
* Bouncyrock.TaleSpire.Runtime (found in Steam\steamapps\common\TaleSpire\TaleSpire_Data\Managed)
* UnityEngine.dll
* UnityEngine.CoreModule.dll
* UnityEngine.InputLegacyModule.dll 
* UnityEngine.UI
* Unity.TextMeshPro
* LordAshes.FAP
```

Build the project.

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```MoreFogColorsPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Changelog
- 1.0.0: Initial release

## Shoutouts
Shoutout to my Patreons on https://www.patreon.com/HolloFox recognising your
mighty contribution to my caffeine addiciton:
- John Fuller
- [Tales Tavern](https://talestavern.com/) - MadWizard