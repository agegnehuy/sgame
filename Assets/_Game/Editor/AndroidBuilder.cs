using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SGame.EditorTools
{
    /// <summary>
    /// One-click Android APK builder for the SafeSteps game.
    ///
    /// Use:
    ///   1. In Unity: SGame → Build Android APK
    ///   2. APK is written to ./Builds/SafeStepsAddis.apk
    ///
    /// The script also exposes a static <see cref="PerformBuild"/> entry point
    /// so the same logic can be invoked from a headless command-line build:
    ///
    ///   unity -batchmode -nographics -quit \
    ///         -projectPath . \
    ///         -executeMethod SGame.EditorTools.AndroidBuilder.PerformBuild
    /// </summary>
    public static class AndroidBuilder
    {
        private const string OutputDir = "Builds";
        private const string ApkName = "SafeStepsAddis.apk";

        [MenuItem("SGame/Build Android APK")]
        public static void BuildFromMenu()
        {
            try
            {
                string apk = BuildApk();
                EditorUtility.DisplayDialog(
                    "Build complete",
                    $"APK written to:\n{apk}\n\nCopy it to your phone, " +
                    "tap to install (allow 'install unknown apps' the first time), " +
                    "then open SafeStepsAddis.",
                    "Open folder");
                EditorUtility.RevealInFinder(apk);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[AndroidBuilder] Build failed: " + e);
                EditorUtility.DisplayDialog(
                    "Build failed",
                    "See Console for full error.\n\n" + e.Message,
                    "OK");
            }
        }

        // Headless / CI entry point. Returns 0 on success, non-zero on failure
        // via System.Environment.Exit so a `unity -batchmode` invocation
        // terminates with the right exit code.
        public static void PerformBuild()
        {
            try
            {
                string apk = BuildApk();
                Debug.Log("[AndroidBuilder] OK → " + apk);
                EditorApplication.Exit(0);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[AndroidBuilder] Build failed: " + e);
                EditorApplication.Exit(1);
            }
        }

        private static string BuildApk()
        {
            // Make sure we're targeting Android. Switching here means the
            // user doesn't have to remember File → Build Profiles → Android.
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.Log("[AndroidBuilder] Switching active build target to Android…");
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                    BuildTargetGroup.Android, BuildTarget.Android);
            }

            // Force the right Android player settings every build. We do this
            // in code (not just via the asset file) because the Editor caches
            // PlayerSettings in memory at startup, so a hand-edit of the
            // ProjectSettings.asset won't take effect for the current session
            // — but setting it through the API does, and is also serialized
            // back to disk on the next save.
            ConfigureAndroidPlayerSettings();

            // Output directory inside the repo. .gitignore should ideally
            // exclude it but we don't enforce that here.
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outDir = Path.Combine(projectRoot, OutputDir);
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
            string apkPath = Path.Combine(outDir, ApkName);

            // Build only the scenes the user has enabled in Build Settings.
            // (Currently just P1_M1_Walk.unity — the other scenes are kept
            // disabled in the list so they still resolve via Tools but
            // don't bloat the APK.)
            var scenes = new System.Collections.Generic.List<string>();
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.enabled) scenes.Add(s.path);
            }
            if (scenes.Count == 0)
            {
                throw new System.Exception(
                    "No enabled scenes in Build Settings. Enable at least one scene " +
                    "(File → Build Settings) and try again.");
            }

            // Bump version code each build so the OS treats it as a real
            // upgrade when reinstalling. The version *name* stays stable
            // unless you explicitly change it.
            PlayerSettings.Android.bundleVersionCode++;

            var options = new BuildPlayerOptions
            {
                scenes = scenes.ToArray(),
                locationPathName = apkPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                // Development build = unsigned debug APK that any phone
                // can sideload + Profiler attach. Switch to BuildOptions.None
                // and provide a keystore for a release build.
                options = BuildOptions.Development | BuildOptions.AllowDebugging,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                throw new System.Exception(
                    $"Build result = {summary.result}. " +
                    $"Errors: {summary.totalErrors}, warnings: {summary.totalWarnings}.");
            }

            Debug.Log($"[AndroidBuilder] Build succeeded: {apkPath} " +
                      $"({summary.totalSize / (1024 * 1024)} MB in {summary.totalTime.TotalSeconds:F1}s)");
            return apkPath;
        }

        /// <summary>
        /// Forces the Android-specific player settings we want for this game.
        /// Called at the start of every build so the configuration is
        /// deterministic regardless of in-memory editor state.
        /// </summary>
        private static void ConfigureAndroidPlayerSettings()
        {
            // IL2CPP is the default Android scripting backend in this project
            // (and what the Play Store ultimately requires). With Unity 6 the
            // bundled NDK + CMake are installed at
            //   .../PlaybackEngines/AndroidPlayer/{NDK,SDK/cmake/3.22.1}/
            // so we don't need to switch to Mono.

            // Enable ARMv7 + ARM64 so IL2CPP doesn't complain about missing
            // 64-bit support and the resulting APK installs on every phone.
            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

            // Lock the game to landscape with auto-rotation between the two
            // landscape sides — the camera was designed for that aspect.
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.useAnimatedAutorotation = true;

            // API 25 = Android 7.1 (covers ~98% of devices in the wild).
            // targetSdkVersion = Auto lets Unity pick the highest installed
            // platform that matches the SDK on disk (currently android-36).
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        }
    }
}
