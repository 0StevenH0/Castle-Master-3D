using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Runs for both Unity's Build button and the CI builder.
public sealed class AndroidBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
            ValidateOfflineAndFullscreen();
    }

    [MenuItem("Castle Master/Build Android APK")]
    public static void Build()
    {
        Directory.CreateDirectory("build/Android");
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        ConfigureAndroidSigningForGitHubActions();

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = "build/Android/Castle-Master-3D.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new BuildFailedException("Android APK build failed: " + report.summary.result);
    }

    private static void ConfigureAndroidSigningForGitHubActions()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true",
                StringComparison.OrdinalIgnoreCase))
            return;

        var keystorePath = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PATH");
        if (string.IsNullOrWhiteSpace(keystorePath))
        {
            // Test/PR builds use Unity's debug signing instead of the project's release keystore setting.
            PlayerSettings.Android.useCustomKeystore = false;
            Debug.Log("GitHub Actions test build: using Android debug signing.");
            return;
        }

        if (!File.Exists(keystorePath))
            throw new BuildFailedException("Android release keystore was not found.");

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = Path.GetFullPath(keystorePath);
        PlayerSettings.Android.keystorePass = RequireEnvironment("ANDROID_KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = RequireEnvironment("ANDROID_KEYALIAS_NAME");
        PlayerSettings.Android.keyaliasPass = RequireEnvironment("ANDROID_KEYALIAS_PASS");
        Debug.Log("GitHub Actions release build: Android signing configured.");
    }

    private static string RequireEnvironment(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
            throw new BuildFailedException("Missing required environment variable: " + name);
        return value;
    }

    [MenuItem("Castle Master/Validate offline shop and fullscreen")]
    public static void ValidateOfflineAndFullscreen()
    {
        var testObject = new GameObject("Offline build validation");
        var preview = new RenderTexture(64, 64, 0);
        try
        {
            var store = testObject.AddComponent<StoreLib>();
            foreach (StoreLib.ProductType type in new[] { StoreLib.ProductType.gem,
                StoreLib.ProductType.gold, StoreLib.ProductType.cmdpts })
            {
                for (int index = 0; index < 2; index++)
                {
                    int calls = 0;
                    store.BuyItem(type, index, (success, actualType, actualIndex) =>
                    {
                        Require(success && actualType == type && actualIndex == index,
                            "Local purchase must return the requested product.");
                        calls++;
                    });
                    Require(calls == 1, "A purchase must complete exactly once.");
                }
            }
            int failures = 0;
            StoreLib.OnResultDelegate reject = (success, type, index) =>
            {
                Require(!success, "Invalid products must not be granted.");
                failures++;
            };
            store.BuyItem(StoreLib.ProductType.max, 0, reject);
            store.BuyItem((StoreLib.ProductType)(-1), 0, reject);
            store.BuyItem(StoreLib.ProductType.gem, -1, reject);
            store.BuyItem(StoreLib.ProductType.gold, 2, reject);
            Require(failures == 4, "Invalid requests must finish without hanging the shop.");
            store.BuyItem(StoreLib.ProductType.gem, 0, null);

            // A nested request must not replace another request's callback or product.
            int nestedCalls = 0;
            store.BuyItem(StoreLib.ProductType.gem, 0, (ok, type, index) =>
                store.BuyItem(StoreLib.ProductType.gold, 1, (nestedOk, nestedType, nestedIndex) =>
                {
                    Require(ok && nestedOk && nestedType == StoreLib.ProductType.gold &&
                        nestedIndex == 1, "Nested local purchase corrupted its result.");
                    nestedCalls++;
                }));
            Require(nestedCalls == 1, "Nested purchase did not complete once.");

            var camera = testObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.orthographic = true;
            camera.orthographicSize = ScreenSize.Height / 2f;
            var fullRect = new Rect(0, 0, 1, 1);
            foreach (float aspect in new[] { 4f / 3f, 1.5f, 16f / 9f, 20f / 9f, 21f / 9f })
            {
                camera.aspect = aspect;
                camera.rect = new Rect(0.15f, 0, 0.7f, 1);
                ScreenSize.AdjustCameraRect(camera);
                ScreenSize.AdjustCameraRect(camera);
                Require(camera.rect == fullRect, "Fullscreen must be repeatable without shrinking.");
                Vector3 edge = camera.ViewportToWorldPoint(new Vector3(1, 1, 10));
                Require(Mathf.Abs(edge.x - 480) < 0.01f && Mathf.Abs(edge.y - 320) < 0.01f,
                    "The full legacy layout must stay visible.");
                Vector3 roundTrip = camera.WorldToViewportPoint(edge);
                Require(Mathf.Abs(roundTrip.x - 1) < 0.001f && Mathf.Abs(roundTrip.y - 1) < 0.001f,
                    "UI rendering and input coordinates must agree.");
            }
            camera.targetTexture = preview;
            camera.aspect = 1;
            camera.rect = new Rect(0.1f, 0.2f, 0.5f, 0.5f);
            var previewRect = camera.rect;
            ScreenSize.AdjustCameraRect(camera);
            Require(camera.rect == previewRect && Mathf.Approximately(camera.aspect, 1),
                "Render-texture previews must keep their own framing.");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(testObject);
            UnityEngine.Object.DestroyImmediate(preview);
        }
        Debug.Log("Offline shop and fullscreen validation passed.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new BuildFailedException(message);
    }
}
