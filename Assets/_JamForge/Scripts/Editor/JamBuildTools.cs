using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JamForge.Editor
{
    public static class JamBuildTools
    {
        [MenuItem("Tools/JamForge/Build Windows")]
        public static void BuildWindows()
        {
            Directory.CreateDirectory("Builds/Windows");
            BuildPlayerOptions options = new()
            {
                scenes = GetEnabledScenes(),
                locationPathName = "Builds/Windows/JamForge.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            Report(BuildPipeline.BuildPlayer(options));
        }

        [MenuItem("Tools/JamForge/Build WebGL")]
        public static void BuildWebGL()
        {
            Directory.CreateDirectory("Builds/WebGL");
            BuildPlayerOptions options = new()
            {
                scenes = GetEnabledScenes(),
                locationPathName = "Builds/WebGL",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            Report(BuildPipeline.BuildPlayer(options));
        }

        private static string[] GetEnabledScenes()
        {
            return System.Array.ConvertAll(
                System.Array.FindAll(EditorBuildSettings.scenes, scene => scene.enabled),
                scene => scene.path);
        }

        private static void Report(BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
                Debug.Log($"Build succeeded: {report.summary.totalSize} bytes.");
            else
                Debug.LogError($"Build failed: {report.summary.result}");
        }
    }
}
