# RoughHouse V5 — Real Human Fighter Rebuild

**Status: reconstructed Unity project/source package. Requires REAL rigged human FBX models and animation clips before a Windows build. Not yet compiled or play-tested.**

The existing V4 project constructs fighters from scaled spheres and rotates them by hand. V5 removes the stick-figure renderer and introduces a human FBX / Unity Animator architecture, grounded CharacterController movement, collision separation, reach-checked strikes, basic hit reactions, ring camera and two demo sequences.

## Fastest option: GitHub + Unity Cloud Build, without local Unity Editor

1. Obtain two **licensed, rigged human models**, in FBX format. They must include humanoid skeleton bones, skinning and avatar-compatible bone layout.
2. Save the models using these EXACT names/paths in your GitHub repository:
   - `Assets/RoughHouse/Imported/Characters/Soho.fbx`
   - `Assets/RoughHouse/Imported/Characters/Slick.fbx`
3. Obtain **12 real FBX animation clips** that use a compatible humanoid rig and contain baked animation. The actual name of the internal animation take does not matter, but the FILE name must match:
   - `Assets/RoughHouse/Imported/Animations/Idle.fbx`
   - `Walk.fbx`, `Jab.fbx`, `Cross.fbx`, `Hook.fbx`, `Uppercut.fbx`, `Kick.fbx`, `Hit.fbx`, `HeavyHit.fbx`, `Knockdown.fbx`, `GetUp.fbx`, `Block.fbx` (all in the same Animations folder).
4. Include any legally distributable accompanying textures/materials and `.meta` files when available. Commit all updated project files to GitHub branch `master`.
5. Start **a new Unity Cloud Windows build**. The editor importer attempts Humanoid rig import and the pre-build setup generates `Resources/Fighters/Soho.prefab`, `Slick.prefab`, and two Animator controllers automatically. The build stops early with an explicit error if the FBX assets are missing, instead of producing a successful but empty game.
6. Download the new Windows artifact, run it and check frame-by-frame contacts, Avatar mapping, foot-floor offset, fighter scale and stance. Rigged model orientation and clip import often require manual retargeting/tuning; an automatic import cannot guarantee realistic results for arbitrary FBX assets.

## Alternative: configure models visually with Unity Editor

Open Unity 6000.3.x, import models and clips, choose **Rig > Animation Type > Humanoid** for each FBX, and use menu **RoughHouse > Configure Human Fighters**. Assign both character models and 12 clip slots; click GENERATE BOTH HUMAN FIGHTERS. Commit generated assets and original source FBX files. The GitHub-only auto importer may regenerate the controllers during cloud build when the standard folder names are present.

## Suggested CC0 sources (download separately)

- Quaternius Universal Base Characters: https://quaternius.com/packs/universalbasecharacters.html
- Quaternius Universal Animation Library: https://quaternius.com/packs/universalanimationlibrary.html

These are stylized/low-poly human assets and will not look photoreal. For photoreal human faces, materials and high-quality choreographed striking, choose appropriate character models and motion-capture animation clips. The licensing of any source assets must be checked before publication.

## Implemented in the V5 code

- No sphere-and-stick model construction; requires two genuine humanoid models.
- Physical floor colliders; grounded capsule controllers; fighter separation and constrained arena movement.
- Close-range choreography, attacks with range and directional checks, pushback, and reaction animations.
- Fighting stance, boxing punches, kicks, knockdown and recovery through real Animator clips.
- Fight camera follows the participants more closely.
- `[1]` lock-on / circling demo, `[2]` combo demo, `[P]` free play, `[Space]` pause; WASD movement and J/K/L/I/U attacks, O block.

## Known limitations

- Exact mocap clip selection and retargeting are not automatic; FBX imports with incompatible skeletons may fail or animate badly.
- No motion blending trees, correct foot IK, or animation-event-authored hit windows yet.
- Character collision uses CharacterController + scripted separation, not full ragdoll simulation.
- Knockdown and get-up currently play clips; advanced synchronized fight animation needs authoring and careful timing.
- The editor has not been executed in this environment. Unity compilation and actual runtime must be verified on the next cloud build.


## V5.1 compilation fix
The Unity 6 built-in Animation module (`com.unity.modules.animation`: `1.0.0`) has been explicitly enabled in `Packages/manifest.json` to resolve CS1069 for `UnityEngine.Animator`. Both Soho and Slick are intended to be male humanoid models; actual licensed character FBX files are not bundled. Cloud builds still require both imported male character assets and the 12 animation FBX files described in ASSET_REQUIREMENTS.txt.
