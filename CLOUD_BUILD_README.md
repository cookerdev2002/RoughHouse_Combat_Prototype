# RoughHouse Unity cloud build (Windows desktop)

This is the first untested **source prototype**, not a verified EXE. Unlike the original ZIP, this edition automatically generates the demo scene on first Unity Editor import and adds it to Build Settings. You do not have to click any menu in the Unity Editor.

## Upload to GitHub

1. Extract this archive. The repository root should contain `Assets`, `Packages`, and `ProjectSettings` (not the surrounding `RoughHouse_Combat_Prototype` folder).
2. On GitHub create a **private** repository (`roughhouse-combat-prototype`). Upload every source item from the extracted project folder using GitHub's **Add file > Upload files**, or use Git. Keep the directory paths intact. **Do not upload** Library, Temp, Logs, Obj, or build outputs. Ensure GitHub shows `Assets/RoughHouse/Scripts/RoughHousePrototype.cs`, `Assets/RoughHouse/Editor/RoughHouseSceneSetup.cs`, `Packages/manifest.json`, `ProjectSettings/ProjectVersion.txt`.
3. Visit https://cloud.unity.com/ and create/select a Unity Cloud Project. Open DevOps > Build Automation. Follow the on-screen onboarding for subscription/trial and source control.
4. Connect GitHub and authorize repository access. Choose the repo above, branch `main`.
5. Create a build configuration: **Windows Desktop / Windows Standalone**, **x86_64**, Unity **6000.3.28f1** (Unity 6.3 LTS) if available. Use a compatible version if that precise editor is not offered, and adjust ProjectVersion accordingly. Project subfolder is blank when Assets/ is at repository root. Disable automatic build triggers while testing.
6. Run **Build**. The Editor importer should generate the scene and register it for the build. If your provider reports no scenes, inspect build logs and ensure the `RoughHouseSceneSetup` editor script compiled and its delayed initializer ran. In that case, generate and commit the scene using a local Unity Editor; an automated build may not call initialization reliably in every configuration.
7. Once successful, download and extract the complete Windows artifact; keep `RoughHouse.exe` alongside its `_Data` folder and Unity runtime DLLs.

## Controls

Startup runs Clip 1 automatically; **2** = Clip 2, **1** = restart Clip 1, **P** = manual/free play, **WASD** movement, **J/K/L/I** attacks, **U** Slick kick, **Space** pause.

## Important

- **No actual video is bundled.** Record the running EXE using OBS or Windows screen recording.
- The procedural character visuals and poses are **temporary placeholders**, not client-approved final character art.
- **Not compiled or tested in Unity yet.** Cloud failures can arise from Unity API compatibility, input settings, package resolution, or scene generation. Review cloud build logs before client delivery.
- The game is a prototype, not the completed commissioned combat game. Test Clip 1 and Clip 2 on a real Windows PC before claiming completion.


## V3 cloud build correction
The repository now includes `Assets/RoughHouse/Scenes/Combat_Test_Arena.unity` and `ProjectSettings/EditorBuildSettings.asset`. Commit **both** to GitHub. This avoids the previous `There were no scenes configured to build` failure caused by relying on automatic scene creation.
