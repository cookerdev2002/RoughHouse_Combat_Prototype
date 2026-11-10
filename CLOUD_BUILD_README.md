# RoughHouse V6 build instructions

Unity 6000.3.x, Build Automation target Windows desktop 64-bit, branch `master`, Unity project subfolder empty. The scene `Assets/RoughHouse/Scenes/Combat_Test_Arena.unity` is included in `ProjectSettings/EditorBuildSettings.asset`.

V6 removes FBX-gating build hooks. If applying V6 to the existing V5.1 GitHub repository **delete the two old editor files** from GitHub Desktop as well as replacing source files:
- `Assets/RoughHouse/Editor/RoughHouseCloudAssets.cs`
- `Assets/RoughHouse/Editor/RoughHouseHumanSetup.cs`

A successful export is not runtime validation. Test the freshly downloaded executable, controls 1 and 2, floor and collisions, and `Player.log` before client presentation. Do not describe generated screenshots as real game footage.
