using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuilderWindow : EditorWindow
{
    string CurrentAPI = "none";
    string BuildVersion = "none";
    string productName = "YourGameName";
    string ScriptingDefineSymbolsForGroup = "CROSS_PLATFORM_INPUT;";

    BuildTarget BuildTargetButton;
    BuildOptions BuildOptionsButton;
    BuildState BuildStateButton = BuildState.Release;

    public bool ShowConfig = false;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("DarckBuilder/OpenWindow")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.

        EditorWindow.GetWindow(typeof(BuilderWindow));
    }

    void OnGUI()
    {
        if (ShowConfig)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Config", EditorStyles.boldLabel);

            if (GUILayout.Button("Back"))
            {
                ShowConfig = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            ScriptingDefineSymbolsForGroup = EditorGUILayout.TextField("ScriptingDefineSymbols", ScriptingDefineSymbolsForGroup);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select Build | Current: " + CurrentAPI, EditorStyles.boldLabel);
            BuildStateButton = (BuildState)EditorGUILayout.EnumPopup(BuildStateButton);

            if (GUILayout.Button("Server"))
            {
                CurrentAPI = "Server";
                BuildVersion = Random.Range(-1500, 1500).GetHashCode().ToString();
                //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "Server;" + ScriptingDefineSymbolsForGroup);
                PlayerSettings.productName = productName + "-Server Build:" + BuildStateButton + " " + System.DateTime.Now.Day + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Year;
            }
            if (GUILayout.Button("Client"))
            {
                CurrentAPI = "Client";

                PlayerSettings.productName = productName + "-Client Build:" + BuildStateButton + " " + System.DateTime.Now.Day + "/" +System.DateTime.Now.Month + "/" + System.DateTime.Now.Year;
            }
            if (GUILayout.Button("Config"))
            {
                ShowConfig = true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label("Current: " + PlayerSettings.productName, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            productName = EditorGUILayout.TextField("productName", productName);
            BuildTargetButton = (BuildTarget)EditorGUILayout.EnumPopup("BuildTargetButton", BuildTargetButton);

            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTargetButton))
            {
                EditorGUILayout.HelpBox("You need Download The " + BuildTargetButton.ToString() + " Module To Build!", MessageType.Error);
                if (GUILayout.Button("How To Download?"))
                {
                    Application.OpenURL("https://docs.unity3d.com/Manual/GettingStartedAddingEditorComponents.html");
                }
                return;
            }

            if (string.IsNullOrEmpty(productName))
            {
                EditorGUILayout.HelpBox("FileName Can't be Empty", MessageType.Error);
                return;
            }

            if (GUILayout.Button("Build " + CurrentAPI))
            {
                BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown/ProjectEvilyn-Client.exe", BuildTargetButton, BuildOptionsButton);
                BuildSummary summary = report.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: Client64");
                }

                if (summary.result == BuildResult.Failed)
                {
                    Debug.Log("Build failed");
                }
            }
        }
    }
}

public enum BuildState
{
    none, Alpha, Beta, Debug, Release
}