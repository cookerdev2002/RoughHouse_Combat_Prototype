# ROUGH HOUSE — Unity Combat Prototype

**Prototype source package for the client's first two short combat videos.**

This is a real, editable Unity C# project scaffold, not an exported Windows game and not recorded gameplay. The package generates a functional **code-driven combat visualization** with procedural stylized placeholder fighters (not final approved character art). It has **not been opened, compiled, or playtested in Unity in this environment**. On first opening, Unity may upgrade packages, ask you to switch the API/input settings or show version-specific warnings; follow the checks below.

## Quick start on Windows 11

1. Install **Unity Hub** and **Unity 6 Editor**. Prefer Unity `6000.0.50f1` (the version recorded in ProjectVersion.txt), or another compatible Unity 6 version.
2. Extract this ZIP to a normal local folder, such as `C:\UnityProjects\RoughHouse_Combat_Prototype`.
3. In Unity Hub select **Add project from disk** and choose the extracted project folder.
4. Open the project using the **3D built-in render pipeline**. The materials in this package use the built-in `Standard` shader. If Hub asks to pick a template instead, simply open the extracted project as-is rather than creating a separate URP project.
5. If the editor asks about **Input Handling**, choose `Both` or `Input Manager (Old)`. The prototype uses `UnityEngine.Input`.
6. In the main menu choose **RoughHouse → Generate Combat Demo Scene**. This saves `Assets/RoughHouse/Scenes/Combat_Test_Arena.unity` and adds it to Build Settings.
7. Click **Play**. Clip 1 runs automatically. Switch to the **Game** tab to see the fighters and HUD.
8. Use the hotkeys below and capture gameplay using OBS Studio or a screen recorder after testing.

## Keyboard controls

| Input | Action |
|---|---|
| `1` | Restart Clip 1 (circling / jab / hit reactions) |
| `2` | Restart Clip 2 (combo / launch / crumple / knockdown / OTG / kicks) |
| `Space` | Pause / resume |
| `P` | Toggle scripted demo / free-play input |
| `W A S D` | Move Soho in free play (fighter keeps facing Slick) |
| `J` | Soho left jab |
| `K` | Soho right jab |
| `L` | Soho hook |
| `I` | Soho uppercut (launch) |
| `U` | Slick's counter kick |

For smooth capture set Game view to 16:9, e.g. 1920 × 1080, and use the Editor's **Maximize On Play** option.

## What this prototype contains

- Two **procedurally assembled original placeholder 3D fighters**, named **Soho** and **Slick**.
- Simple low-poly arena with boundary markings, lighting and contrasting backdrops.
- Scripted fighting stance, circular lock-on movement, two-character following camera and camera impact shake.
- Soho jabs, hook and uppercut; opponent hit, airborne launch, crumple, ground impact and recovery poses.
- Slick quick and spinning/tricking-style kick motions.
- Scripted demo timelines; alternate free-play keyboard mode.
- On-screen demo labels and recording shortcuts.

## Important limitations / quality gates

These procedural 3D models are **not final approved Soho or Slick models**: the provided client text gives character personalities and fighting styles but no approved concept art or mesh references. Movements are **procedural pose animation**, not motion-captured or hand-keyed final animation. The demo has no genuine hitbox/hurtbox collision, frame data, network play, health system, sound effects, audio, custom skinned meshes or production animation controller. For real clips, verify body motion/spacing in Unity and adjust the script as needed. The hit reaction is timed to the scripted swing rather than contact physics. Do not claim final polished combat is complete based solely on this scaffold.

The project **does not include an MP4**. A real gameplay video requires running the Unity scene on a computer with the Editor/renderer available and recording it.

## Proposed client approval steps

1. **Milestone A: Clip 1** — confirm a 20–30 second fight-stance/circling/camera/hit-reaction clip.
2. **Milestone B: Clip 2** — confirm the jab → jab → hook → uppercut launch, crumple, grounded hit, knockdown/get-up and Slick fast kicks.
3. **Art polish** — replace placeholder bodies with approved rigged Soho/Slick models, proper animations, VFX, SFX and any brand-specific stage assets.
4. **Final handoff** — the Unity project and all newly created/licensed assets, plus actual recorded combat clips.

## Project files

- `Assets/RoughHouse/Scripts/RoughHousePrototype.cs` — stage creation, procedural character geometry, pose animations, camera, clip director, inputs, HUD.
- `Assets/RoughHouse/Editor/RoughHouseSceneSetup.cs` — one-click Unity Editor setup menu.
- `Assets/RoughHouse/Scenes` — generated scene is saved here when the Editor setup menu runs.
- `Packages/manifest.json` and `ProjectSettings/ProjectVersion.txt` — project compatibility metadata.

No third-party asset downloads or license claims are included; graphics are generated from standard Unity primitives and C#.
