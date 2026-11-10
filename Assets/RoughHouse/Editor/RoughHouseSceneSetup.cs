#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class RoughHouseSceneSetup
{
    // Cloud builders cannot click the RoughHouse setup menu. Generate the demo
    // scene automatically on first editor import, then add it to build settings.
    static RoughHouseSceneSetup()
    {
        EditorApplication.delayCall += EnsureCloudBuildScene;
    }

    private static void EnsureCloudBuildScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            return;

        string destination = "Assets/RoughHouse/Scenes/Combat_Test_Arena.unity";
        if (System.IO.File.Exists(destination))
        {
            EnsureBuildSettings(destination);
            return;
        }

        // Do not replace any scene the user is actively editing.
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
            return;

        CreateScene(destination, false);
    }

    private static void EnsureBuildSettings(string destination)
    {
        if (EditorBuildSettings.scenes.Length == 1 &&
            EditorBuildSettings.scenes[0].path == destination &&
            EditorBuildSettings.scenes[0].enabled)
            return;
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(destination, true) };
    }

    private static void CreateScene(string destination, bool interactive)
    {
        System.IO.Directory.CreateDirectory("Assets/RoughHouse/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("ROUGH HOUSE / Scene Bootstrap").AddComponent<RoughHousePrototype>();
        EditorSceneManager.SaveScene(scene, destination);
        EnsureBuildSettings(destination);
        AssetDatabase.SaveAssets();
        Debug.Log("RoughHouse: Combat demo scene generated for Windows cloud build.");
        if (interactive)
        {
            Selection.activeGameObject = GameObject.Find("ROUGH HOUSE / Scene Bootstrap");
            EditorUtility.DisplayDialog("RoughHouse Ready", "Demo scene created. Press Play to view Clip 1. Press 2 for Clip 2.", "OK");
        }
    }

    [MenuItem("RoughHouse/Generate Combat Demo Scene")]
    public static void Generate()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Stop Play Mode before generating the scene.");
            return;
        }
        CreateScene("Assets/RoughHouse/Scenes/Combat_Test_Arena.unity", true);
    }
}
#endif
