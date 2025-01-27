using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MedievalRacingBuildPipeline
{
    private static readonly string[] SCENES_CLIENT =
    {
        "Assets/Scenes/Menu.unity",
        "Assets/Scenes/RoomScenes/RoomOffline.unity",
        "Assets/Scenes/RoomScenes/RoomOnline.unity",
        "Assets/Scenes/Spline/Circuit.unity",
    };

    private static readonly string[] SCENES_SERVER =
    {
        "Assets/Scenes/RoomScenes/RoomOffline.unity",
        "Assets/Scenes/RoomScenes/RoomOnline.unity",
        "Assets/Scenes/Spline/Circuit.unity",
    };

    private static readonly string DEFAULT_BUILD_REPO = "Builds";
    private static readonly string HEADLESS_DEFAULT_SERVER_BUILD = "Builds/Server";

    [MenuItem("File/Custom Build/Client Build")]
    public static void BuildGameForWindows()
    {
        var path = EditorUtility.SaveFolderPanel("Choose Client Build Location", DEFAULT_BUILD_REPO, "");
        Directory.CreateDirectory(path + "/Client");

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES_CLIENT,
            locationPathName = path + "/Client/MedievalRacingClient.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        };

        ReportBuild(BuildPipeline.BuildPlayer(buildPlayerOptions));
    }

    [MenuItem("File/Custom Build/Server Build")]
    public static void BuildServerForLinux()
    {
        var path = EditorUtility.SaveFolderPanel("Choose Server Build Location", DEFAULT_BUILD_REPO, "");
        Directory.CreateDirectory(path + "/Server");

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES_SERVER,
            locationPathName = path + "/Server/MedievalRacingServer.exe",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.None,
            subtarget = (int)StandaloneBuildSubtarget.Server
        };

        ReportBuild(BuildPipeline.BuildPlayer(buildPlayerOptions));
    }

    public static void HeadlessBuildServerForLinux()
    {
        Directory.CreateDirectory(HEADLESS_DEFAULT_SERVER_BUILD);

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES_SERVER,
            locationPathName = HEADLESS_DEFAULT_SERVER_BUILD + "/MedievalRacingServer.exe",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.None,
            subtarget = (int)StandaloneBuildSubtarget.Server
        };

        ReportBuild(BuildPipeline.BuildPlayer(buildPlayerOptions));
    }

    private static void ReportBuild(BuildReport report)
    {
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}
