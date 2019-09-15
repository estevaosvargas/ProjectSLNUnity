using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildingGame : MonoBehaviour {

    #region ClientBuild
    [MenuItem("BuildClient/ClientWin-64b", false, 1)]
    private static void NewMenuOption()
    {
        chanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown/2DTopDown.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
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

    [MenuItem("BuildClient/ClientWin-64b(DevBuild)", false, 1)]
    private static void NewMenuOptionBuild()
    {
        chanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown/2DTopDown.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: Client64(DEV)");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("BuildClient/ClientWin-32b", false, 1)]
    private static void ClientWindows32()
    {
        chanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown32/2DTopDown.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: Client32");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("BuildClient/ClientMac", false, 2)]
    private static void NewMenuOption2()
    {
        Debug.Log("Client Version For MAC is not Ready");
    }

    [MenuItem("BuildClient/ClientLinux", false, 3)]
    private static void NewMenuOption3()
    {
        Debug.Log("Client Version For Linux is not Ready");
    }

    [MenuItem("BuildClient/ClientAndroid", false, 3)]
    private static void AndroidBuild()
    {
        chanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown.apk", BuildTarget.Android, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: Android");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    #endregion

    #region ClientBuild-Demo
    [MenuItem("BuildClient/Demo/ClientWin-64b", false, 1)]
    private static void ClientWindows64Demo()
    {
        DemochanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown/2DTopDown.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
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

    [MenuItem("BuildClient/Demo/ClientWin-32b", false, 1)]
    private static void ClientWindows32Demo()
    {
        DemochanageClient();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDown32/2DTopDown.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: Client32");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("BuildClient/Demo/ClientMac", false, 2)]
    private static void ClientMacDemo()
    {
        Debug.Log("Client Version For MAC is not Ready");
    }

    [MenuItem("BuildClient/Demo/ClientLinux", false, 3)]
    private static void ClientLinuxDemo()
    {
        Debug.Log("Client Version For Linux is not Ready");
    }
    #endregion

    #region ServerBuild
    [MenuItem("BuildServer/ServerWin32", false, 4)]
    private static void NewMenuOption4()
    {
        chanageServer();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDownServer32/2DTopDownServer.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: ServerWin32");
            chanageClient();
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("BuildServer/ServerWin64", false, 10)]
    private static void NewMenuOption8()
    {
        chanageServer();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDownServer/2DTopDownServer.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: ServerWin64");
            chanageClient();
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("BuildServer/ServerLinux", false, 5)]
    private static void NewMenuOption5()
    {
        chanageServer();
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "C:/Users/estev/Desktop/2DTopDownServer/", BuildTarget.StandaloneLinux, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes" + " | Type: ServerLinux");
            chanageClient();
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    #endregion

    private static void chanageServer()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        scenes[0].enabled = false;
        scenes[1].enabled = false;
        scenes[2].enabled = true;
        scenes[3].enabled = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;" + "Server;");

        Debug.Log("Building Server...");

        EditorBuildSettings.scenes = scenes;
    }

    private static void chanageClient()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        scenes[0].enabled = true;
        scenes[1].enabled = true;
        scenes[2].enabled = false;
        scenes[3].enabled = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;" + "Client;");

        Debug.Log("Building Client...");

        EditorBuildSettings.scenes = scenes;
    }

    private static void DemochanageClient()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        scenes[0].enabled = true;
        scenes[1].enabled = true;
        scenes[2].enabled = false;
        scenes[3].enabled = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT; Client; Demo;");

        Debug.Log("Building Demo Client...");

        EditorBuildSettings.scenes = scenes;
    }
}
