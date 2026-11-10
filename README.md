# RoughHouse V6 – self-contained playable combat prototype

This Unity 6.3 source is intended for the existing Windows Cloud Build pipeline. It includes **two male human-proportioned procedural fighters**, a physical arena floor, close-range fighting choreography, camera follow, combat contact/range tests, recoil and hit response, and a keyboard-controlled clip showcase.

**Honest delivery status:** The character meshes are stylized originals assembled from Unity primitives; animation is **procedural**, not motion-captured, and this is **not a realistic/photorealistic human fighting game**. There are no unlicensed character assets or fake FBX files. A client expecting realistic models/animation will still require licensed rigged male characters, retargeted motion-capture, animation blending, animation-event hitboxes, and quality assurance. Build and gameplay have not been tested with the Unity editor in this environment.

## Running

1. Extract the ZIP **contents** into the existing `RoughHouse_Combat_Prototype` repository (replace matching files).
2. Crucial: delete old `Assets/RoughHouse/Editor/RoughHouseCloudAssets.cs` and `RoughHouseHumanSetup.cs` from the Git repository. (They were removed from this package but copying over an old repo doesn't delete them.)
3. Commit and push `master` in GitHub Desktop.
4. Start a NEW Unity Cloud Windows 64-bit Build Automation build on `master`.
5. Download artifacts, extract the complete Windows player, launch it and inspect gameplay.

## Game controls

- `1` – boxing/circling showcase
- `2` – jab, jab, hook, uppercut, knockdown, recovery and kicks
- `P` – free-play mode
- `WASD` – movement in free-play
- `J/K/L/I` – jab / cross / hook / uppercut
- `U` – Slick kick
- `O` – guard
- `Space` – pause/resume

## Important limitations

This is an intermediate playable prototype intended to replace earlier broken or asset-blocked builds, **not a client-ready realistic fighting game**. The user must test the generated Windows executable; the source was inspected and packaged but not compiled or played in Unity here. The implementation intentionally uses two visible male-styled characters with more human proportion and authored fight poses, not the previous sphere-stick fighters, but low-complexity meshes remain visible. Ground contact is enforced by a planar mover and spacing solver; gameplay does not have robust rigidbody dynamics, animation warping, IK foot placement or motion capture.

## V6.1 module correction
Explicitly enables Unity built-in Audio, IMGUI and legacy Input modules for the camera listener, GUI overlay and keyboard controls. Compiling in Unity Cloud and runtime validation are still pending.
