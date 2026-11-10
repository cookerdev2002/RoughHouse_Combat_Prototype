# RoughHouse V5 Unity Cloud Build

Windows Build Automation configuration: platform Windows desktop x86-64, Unity 6000.3.x, Git branch `master`, project root blank, same as successful v4 build.

**IMPORTANT: Unlike V4, V5 intentionally refuses to build without real humanoid models and all 12 animation files.** Otherwise the result would mislead by producing another successful executable without fighters.

Add real FBX assets at the exact paths in `ASSET_REQUIREMENTS.txt`; the `RoughHouseCloudAssets` editor script imports model humanoid rigs and generates two human prefabs + Animator controllers before the cloud build. The character source and clips must be compatible; mismatched bones/clip contents require adjustment in Unity Editor.

Character and animation resources are not included in this ZIP. Use the linked CC0 sources or other licensed FBX models. Download manually and verify usage rights.

After committing assets to Git, run a NEW build. If a BuildFailedException says an asset is missing, upload it at the required path. When cloud compilation succeeds, download the artifact and inspect gameplay; do not assume successful compilation implies acceptable fighter choreography.
