#if ADS
using IPS.Api.Ads;
#endif
using IPS.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildScript {
    [MenuItem("Custom Utilities/Build IOS")]
    static void PerformBuildIOS() {
        BuildPipeline.BuildPlayer(GetEnabledScenes(), "./builds/IOS",
          BuildTarget.iOS, BuildOptions.None);
    }

    static string[] GetEnabledScenes() {
        return (
          from scene in EditorBuildSettings.scenes
          where scene.enabled
          where !string.IsNullOrEmpty(scene.path)
          select scene.path
        ).ToArray();
    }


    [MenuItem("Custom Utilities/Build Android")]
    static void PerformBuildAndroid() {

        PlayerSettings.Android.keystoreName = $"{Application.identifier}.keystore";
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasName = $"{Application.identifier}";
        PlayerSettings.Android.keyaliasPass = "123456";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // target android
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        // setup enabled scenes
        buildPlayerOptions.scenes = GetEnabledScenes();

        // SetScriptingBackend for IL2CPP
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        // Support ARMv7 and ARMv64
        AndroidArchitecture aac = AndroidArchitecture.ARM64;
        aac |= AndroidArchitecture.ARMv7;
        PlayerSettings.Android.targetArchitectures = aac;

        SetBuildVersionFromParam();
        // app name

        buildPlayerOptions.locationPathName = BuildName();

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"Failed to build Android with {buildReport.summary.result} status");

        var summary = buildReport.summary;
        Console.WriteLine("Succeed to create " + buildPlayerOptions.locationPathName + " size: " + summary.totalSize);
    }

    static void SetBuildVersionFromParam() {
        if (GetEnv(IsAAB, out var isaab)) {
            Debug.Log($"isAAB.. value: {is_production}");
            is_aab = isaab == "true";
            is_production = is_aab;
            EditorUserBuildSettings.buildAppBundle = is_production;
            UpdateSymbols(new KeyValuePair<string, bool>("PRODUCTION", is_production));
        }

        if (!is_production && GetEnv(ProductionBuild, out var param)) {
            is_production = param == "true";
            UpdateSymbols(new KeyValuePair<string, bool>("PRODUCTION", is_production));
        }

        if (GetEnv(RemoveAdsBuild, out var paramRemoveAds)) {
            is_removeads = !is_production && paramRemoveAds == "true";
            UpdateSymbols(new KeyValuePair<string, bool>("NOAD", is_removeads));
        }

        Debug.Log("Set up parameter");
        if (GetEnv(BundleKey, out var bundle)) {
            int.TryParse(bundle, out var bundleID);
            if (bundleID != -1) {
                PlayerSettings.Android.bundleVersionCode = bundleID;
            }
        }

        if (GetEnv(VersionKey, out var buildVersion)) {
            if (buildVersion != "") {
                PlayerSettings.bundleVersion = buildVersion;
            }
        }

        Debug.Log($"DevelopmentBuild.. contain key: {GetEnv(DevelopmentBuild, out var a)}");
        Debug.Log($"DevelopmentBuild.. value: {a}");
        if (GetEnv(DevelopmentBuild, out var isDevelopmentBuild)) {
            EditorUserBuildSettings.development = isDevelopmentBuild == "true";
            UpdateSymbols(new KeyValuePair<string, bool>("CUSTOM_DEBUG", isDevelopmentBuild == "true"));
        }

#if ADS
        if (GetEnv(UseAdTest, out var result)) {
            useAdTest = !is_aab && result == "true";
            if (useAdTest) {
                AdmobEditor.SaveConfigForAdTest();
#if MAX
                MaxSettingsEditor.SaveMaxConfig();
#endif
#if IS
                IronSourceSettingsEditor.SaveConfig();
#endif
            }
        }
#endif
            }
    
    static string BuildName() {
        string debug = is_production ? "production" : "cheat";
        if (is_removeads) debug += "_noads";
        if (useAdTest) debug += "_adtest";
        if (EditorUserBuildSettings.development) debug += "_fulllog";

        return $"{Application.productName.Replace(" ", string.Empty)}_v{Application.version}" +
               $"_c{PlayerSettings.Android.bundleVersionCode}" +
               $"_{debug}_{DateTime.Now:ddMMyyyy_HHmm}." +
               (is_aab ? "aab" : "apk");
    }

    private static BuildTargetGroup Platform {
        get {
#if UNITY_ANDROID
            return BuildTargetGroup.Android;
#elif UNITY_IOS
                return BuildTargetGroup.iOS;
#else
                return BuildTargetGroup.Standalone;
#endif
        }
    }
    public static void UpdateSymbols(params KeyValuePair<string, bool>[] symbols) {
        if (symbols == null || symbols.Length == 0) return;

        var list = GetCurrentSymbols();

        foreach (var item in symbols) {
            UpdateFlag(list, item.Key, item.Value);
        }

        SaveSymbols(list);
    }

    static List<string> GetCurrentSymbols() {
        string flagString = PlayerSettings.GetScriptingDefineSymbolsForGroup(Platform);
        return new List<string>(flagString.Split(';'));
    }

    static void SaveSymbols(List<string> list) {
        string newFlags = string.Join(";", list);
        Debug.Log($"SetScriptingDefineSymbols: {newFlags}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(Platform, newFlags);
    }

    static void UpdateFlag(List<string> currentList, string flag, bool enable) {
        if (enable && !currentList.Contains(flag)) {
            currentList.Add(flag);
        }
        else if (!enable && currentList.Contains(flag)) {
            currentList.Remove(flag);
        }
    }

    static void BuildIOS() {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // app name
        buildPlayerOptions.locationPathName = BuildName();

        // target iOS
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        // setup enabled scenes
        buildPlayerOptions.scenes = GetEnabledScenes();

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"Failed to build IOS with {buildReport.summary.result} status");

        var summary = buildReport.summary;
        Console.WriteLine("Succeed to create " + buildPlayerOptions.locationPathName + " outputPath: " + summary.outputPath);
    }


    static bool GetEnv(string key, out string value) {
        value = Environment.GetEnvironmentVariable(key);
        return !string.IsNullOrEmpty(value);
    }

    // Version
    private const string VersionKey = "version";
    private const string BundleKey = "bundle_id";
    private const string RemoveAdsBuild = "removeAdsBuild";
    private const string UseAdTest = "useAdTest";
    private const string DevelopmentBuild = "developmentBuild";
    private const string ProductionBuild = "productionBuild";
    private const string IsAAB = "AAB";
    private static bool is_aab = false;
    private static bool is_production = false;
    private static bool is_removeads = false;
    private static bool useAdTest = false;

}