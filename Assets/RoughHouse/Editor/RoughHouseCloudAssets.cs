#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Enables a GitHub-only workflow: upload raw FBX files into deterministic paths,
// Unity Cloud Editor can import and assemble the characters before the Windows build.
public sealed class RoughHouseCloudAssets : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return -2000; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        string characters="Assets/RoughHouse/Imported/Characters";
        string animations="Assets/RoughHouse/Imported/Animations";
        string a=characters+"/Soho.fbx", b=characters+"/Slick.fbx";
        if(!File.Exists(a) || !File.Exists(b))
            throw new BuildFailedException("RoughHouse V5 requires REAL humanoid models at " + a + " and " + b + ". Add actual licensed FBX character files to GitHub first. See ASSET_REQUIREMENTS.txt.");
        GameObject first=AssetDatabase.LoadAssetAtPath<GameObject>(a);
        GameObject second=AssetDatabase.LoadAssetAtPath<GameObject>(b);
        if(first==null || second==null)
            throw new BuildFailedException("RoughHouse V5 cannot load humanoid FBX models from Assets/RoughHouse/Imported/Characters. Verify both models were imported.");
        string[] names={"Idle","Walk","Jab","Cross","Hook","Uppercut","Kick","Hit","HeavyHit","Knockdown","GetUp","Block"};
        AnimationClip[] clips=new AnimationClip[names.Length];
        for(int i=0;i<names.Length;i++)
        {
            string fbx=animations+"/"+names[i]+".fbx";
            if(!File.Exists(fbx))
                throw new BuildFailedException("RoughHouse V5: missing REAL humanoid animation: "+fbx+". Each filename must match the required animation slot.");
            foreach(UnityEngine.Object o in AssetDatabase.LoadAllAssetsAtPath(fbx))
            {
                AnimationClip c=o as AnimationClip;
                if(c!=null && !c.name.StartsWith("__preview__") && !c.name.StartsWith("Take 001_preview")) { clips[i]=c; break; }
            }
            if(clips[i]==null) throw new BuildFailedException("RoughHouse V5 cannot find an imported AnimationClip in "+fbx+". FBX should contain baked keyframe animation.");
        }
        string root="Assets/Resources/Fighters";
        Directory.CreateDirectory(root);
        RoughHouseHumanSetup.BuildOne("Soho",first,names,clips,root);
        RoughHouseHumanSetup.BuildOne("Slick",second,names,clips,root);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("RoughHouse V5: REAL humanoid fighters and 12 animation states assembled successfully for Cloud Build.");
    }
}

public sealed class RoughHouseModelImport : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        if(!assetPath.StartsWith("Assets/RoughHouse/Imported/",StringComparison.OrdinalIgnoreCase)) return;
        ModelImporter model=assetImporter as ModelImporter;
        if(model==null) return;
        model.animationType=ModelImporterAnimationType.Human;
        model.importAnimation = true;
    }
}
#endif
