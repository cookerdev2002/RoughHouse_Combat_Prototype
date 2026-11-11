ROUGH HOUSE V6.4 -- TARGETED PATCH FOR THE REPEATED V5 BUILD ERROR

This patch only neutralizes the two old V5 scripts that were still present
in your GitHub repository's master branch. It does not replace your V6 fighter
controller, scene, package manifest, or shader.

INSTALL:
1) Extract this ZIP.
2) Copy the Assets folder from this ZIP into the ROOT folder of your cloned
   RoughHouse_Combat_Prototype repository, merging directories and REPLACING
   both existing .cs files. DO NOT copy the ZIP folder itself.
3) In GitHub Desktop's Changes tab verify TWO modified .cs files:
     Assets/RoughHouse/Editor/RoughHouseCloudAssets.cs
     Assets/RoughHouse/Editor/RoughHouseHumanSetup.cs
   Open the first diff: the old BuildFailedException requiring Soho.fbx and
   Slick.fbx must appear removed (red), replaced by comment-only source.
4) Commit on master, then click PUSH ORIGIN.
5) Verify on the GitHub WEBSITE that RoughHouseCloudAssets.cs contains only
   these patch comments, NOT the V5 BuildFailedException.
6) In Unity Cloud Build History start a new build and confirm its COMMIT SHA
   matches the GitHub master branch's latest commit. Do not Replay old builds.

These two comment-only C# files compile harmlessly and cannot stop the build.
The old V5 compiler/build hooks are deliberately absent. Other unrelated
build or runtime errors, if any, still require validation with Unity Cloud.

NOTE: RoughHouse V6 still contains procedural human-proportioned fighters, not
photo-realistic human meshes or motion-capture animation. Do not represent this
as a finished realistic fighting game to the client.
