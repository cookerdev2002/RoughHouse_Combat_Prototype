#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

// Import utility: all inputs are real FBX assets chosen by the user.
// The tool does not download third-party assets or invent humanoid animations.
public sealed class RoughHouseHumanSetup : EditorWindow
{
    private GameObject sohoSource, slickSource;
    private AnimationClip idle, walk, jab, cross, hook, uppercut, kick, hit, heavyHit, knockdown, getUp, block;
    private Vector2 scroll;
    [MenuItem("RoughHouse/Configure Human Fighters")]
    public static void Open() { GetWindow<RoughHouseHumanSetup>("RoughHouse Human Setup"); }
    private void OnGUI()
    {
        scroll=EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.HelpBox("Drag two actual humanoid FBX models and matching humanoid animations here. Configure imported FBX Rig > Animation Type = Humanoid; Apply. This wizard builds the Resources prefabs and AnimatorControllers used by cloud builds.", MessageType.Info);
        sohoSource=(GameObject)EditorGUILayout.ObjectField("Soho human FBX / prefab",sohoSource,typeof(GameObject),false);
        slickSource=(GameObject)EditorGUILayout.ObjectField("Slick human FBX / prefab",slickSource,typeof(GameObject),false);
        EditorGUILayout.Space();
        idle=Pick("Fighting idle",idle); walk=Pick("Footwork / walk",walk); jab=Pick("Left jab",jab);
        cross=Pick("Right cross",cross); hook=Pick("Hook",hook); uppercut=Pick("Uppercut",uppercut);
        kick=Pick("Quick kick",kick); hit=Pick("Hit reaction",hit); heavyHit=Pick("Heavy hit",heavyHit);
        knockdown=Pick("Knockdown",knockdown); getUp=Pick("Get up",getUp); block=Pick("Block",block);
        EditorGUILayout.Space();
        if(GUILayout.Button("GENERATE BOTH HUMAN FIGHTERS",GUILayout.Height(42))) Generate();
        EditorGUILayout.EndScrollView();
    }
    private static AnimationClip Pick(string label,AnimationClip val) { return (AnimationClip)EditorGUILayout.ObjectField(label,val,typeof(AnimationClip),false); }
    private void Generate()
    {
        if(sohoSource==null || slickSource==null || idle==null || walk==null || jab==null || cross==null || hook==null || uppercut==null || kick==null || hit==null || heavyHit==null || knockdown==null || getUp==null || block==null)
        {
            EditorUtility.DisplayDialog("Missing human assets", "Assign BOTH rigged fighter models and all 12 motion clips before generating. Missing clips must be sourced from a properly licensed humanoid animation pack.", "OK"); return;
        }
        string[] names={"Idle","Walk","Jab","Cross","Hook","Uppercut","Kick","Hit","HeavyHit","Knockdown","GetUp","Block"};
        AnimationClip[] clips={idle,walk,jab,cross,hook,uppercut,kick,hit,heavyHit,knockdown,getUp,block};
        string root="Assets/Resources/Fighters";
        Directory.CreateDirectory(root);
        try
        {
            BuildOne("Soho",sohoSource,names,clips,root);
            BuildOne("Slick",slickSource,names,clips,root);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Human fighters ready", "Both prefabs and animation controllers are generated. Commit the new Resources/Fighters assets plus their .meta files to GitHub, then build using Unity Cloud.","OK");
        }
        catch(System.Exception ex) { Debug.LogException(ex); EditorUtility.DisplayDialog("Generation error",ex.Message,"OK"); }
    }
    internal static void BuildOne(string label,GameObject source,string[] names,AnimationClip[] clips,string root)
    {
        string controllerPath=root+"/"+label+".controller";
        if(AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath)!=null)
            AssetDatabase.DeleteAsset(controllerPath);
        var controller=AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var layer=controller.layers[0];
        var machine=layer.stateMachine;
        for(int i=0;i<names.Length;i++)
        {
            var state=machine.AddState(names[i]);
            state.motion=clips[i];
            state.writeDefaultValues=true;
            if(i==0) machine.defaultState=state;
        }
        var instance=(GameObject)PrefabUtility.InstantiatePrefab(source);
        if(instance==null) instance=Instantiate(source);
        instance.name=label;
        instance.transform.position=Vector3.zero;
        instance.transform.rotation=Quaternion.identity;
        var animator=instance.GetComponentInChildren<Animator>();
        if(animator==null) animator=instance.AddComponent<Animator>();
        animator.runtimeAnimatorController=controller;
        animator.applyRootMotion=false;
        animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
        string prefabPath=root+"/"+label+".prefab";
        PrefabUtility.SaveAsPrefabAsset(instance,prefabPath);
        DestroyImmediate(instance);
        Debug.Log("RoughHouse V5: generated human fighter prefab at "+prefabPath);
    }
}
#endif
