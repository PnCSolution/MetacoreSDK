#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using AndroidArchitecture = UnityEditor.AndroidArchitecture;

#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
public static class MetacoreProjectValidation
{
    public static readonly BuildTargetGroup[] BuildTargetGroups = {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android,
    };


    private const string Category = "MetaCore";
    private const string XRProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
    private static List<BuildValidationRule> validationRules = new List<BuildValidationRule>();


    [InitializeOnLoadMethod]
    private static void ShowWelcomePopupUI()
    {
        // get version prom package.json 
        PackageInfo packageInfo = PackageInfo.FindForAssetPath("Packages/com.pncsolution.metacore");
        string version = PlayerPrefs.GetString(Category + "Version", "");
        if (string.IsNullOrEmpty(version))
        {
            // show ui
            //Debug.Log("show UI!");
            //PlayerPrefs.SetString(Category + "Version", packageInfo.version);
        }
    }

    [InitializeOnLoadMethod]
    private static void AddInputValidationRule()
    {
        var allRules = new List<BuildValidationRule>();
        allRules.AddRange(GenerateSpacesRules());
        allRules.AddRange(GenerateMRTKRules());
        allRules.AddRange(GeneratePlatformRules());
        allRules.AddRange(GeneratePlayerSettingRules());

        AddTargetDependentRules(allRules);  // Call BuildValidator.AddRules only once
    }

    private static List<BuildValidationRule> GeneratePlatformRules()
    {
        List<BuildValidationRule> rules = new List<BuildValidationRule>();
        // target platform  is Android
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () => EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android,
                Category = Category,
                Message = "The target platform must be set to Android.",
                CheckPredicate = () => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android,
                FixIt = () => EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android),
                FixItMessage = "Set the target platform to Android",
                FixItAutomatic = false,
                Error = true,
            };
            rules.Add(rule);
        }

        // if using mono, change il2cpp
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () => PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP,
                Category = Category,
                Message = "The Android Scripting Backend must be set to IL2CPP.",
                CheckPredicate = () => PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP,
                FixIt = () => PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP),
                FixItMessage = "Set the Android Scripting Backend to IL2CPP",
                FixItAutomatic = false,
                Error = true,
            };
            rules.Add(rule);
        }

        return rules;
    }

    private static List<BuildValidationRule> GeneratePlayerSettingRules()
    {
        List<BuildValidationRule> rules = new List<BuildValidationRule>();
        // Check orientation is LandscapeLeft
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () => PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeLeft,
                CheckPredicate = () => PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeLeft,
                Category = Category,
                Message = "Default orientation must be Landscape Left",
                FixIt = () => PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft,
                FixItMessage = "Set default orientation to Landscape Left",
                FixItAutomatic = true,
                Error = false,
            };
            rules.Add(rule);
        }

        // Check Target Architectures
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () =>
                {
                    var arch = PlayerSettings.Android.targetArchitectures;
                    return !arch.HasFlag(AndroidArchitecture.ARM64) || arch.HasFlag(AndroidArchitecture.ARMv7);
                },
                CheckPredicate = () =>
                {
                    var arch = PlayerSettings.Android.targetArchitectures;
                    return arch.HasFlag(AndroidArchitecture.ARM64) && !arch.HasFlag(AndroidArchitecture.ARMv7);
                },
                Category = Category,
                Message = "Target Architectures must include ARM64 only",
                FixIt = () =>
                {
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                },
                FixItMessage = "Set Target Architectures to ARM64 only",
                FixItAutomatic = true,
                Error = true,
            };
            rules.Add(rule);
        }

        // Undsafe Code Rule
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () => !PlayerSettings.allowUnsafeCode,
                CheckPredicate = () => PlayerSettings.allowUnsafeCode,
                Category = Category,
                Message = "The 'Allow 'unsafe' code' option must be enabled.",
                FixIt = () => PlayerSettings.allowUnsafeCode = true,
                FixItMessage = "Enable 'Allow unsafe code'",
                FixItAutomatic = true,
                Error = true,
            };
            rules.Add(rule);
        }

        // Check PackageName
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () =>
                {
                    string currentId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                    return string.IsNullOrEmpty(currentId) || !currentId.StartsWith("com.metalense.");
                },
                CheckPredicate = () =>
                {
                    string currentId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                    return currentId.StartsWith("com.metalense.");
                },
                Category = Category,
                Message = "Override Default Package Name must be enabled and start with 'com.metalense.'",
                FixIt = () =>
                {
                    string productName = Application.productName.ToLowerInvariant().Replace(" ", "");
                    string newId = $"com.metalense.{productName}";
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, newId);
                    Debug.Log($"Pakage Name set to: {newId}");
                },
                FixItMessage = "Enable Override Package Name and set to 'com.metalense.{productname}'",
                FixItAutomatic = true,
                Error = true,
            };
            rules.Add(rule);
        }

        // Check CompanyName
        {
            BuildValidationRule rule = new BuildValidationRule()
            {
                IsRuleEnabled = () => PlayerSettings.companyName != "metalense",
                CheckPredicate = () => PlayerSettings.companyName == "metalense",
                Category = Category,
                Message = "Company Name must be 'metalense'",
                FixIt = () => PlayerSettings.companyName = "metalense",
                FixItMessage = "Set Company Name to 'metalense'",
                FixItAutomatic = true,
                Error = true,
            };
            rules.Add(rule);
        }

        return rules;
    }

    private static List<BuildValidationRule> GenerateMRTKRules()
    {
        List<BuildValidationRule> rules = new List<BuildValidationRule>();
        {
            string[] packageNames = new string[] { "com.microsoft.mixedreality.openxr",
                //"com.microsoft.mixedreality.toolkit" 
            };
            string helpLink = "https://learn.microsoft.com/ko-kr/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/setting-up/setup-new-project";

            for (int i = 0; i < packageNames.Length; i++)
            {
                string packageName = packageNames[i];
                string message = "The " + packageName + " package is not installed. Please install it from the SDK Website";
                rules.Add(GenerateCheckExistPakageRule(packageName, message, helpLink));
            }
        }

        return rules;
    }

    public static List<BuildValidationRule> GenerateSpacesRules()
    {
        List<BuildValidationRule> rules = new List<BuildValidationRule>();
        string[] packageNames = new string[] { "com.qualcomm.snapdragon.spaces", "com.qualcomm.qcht.unity.interactions" };
        string helpLink = "https://docs.spaces.qualcomm.com/unity/setup/setup-guide";

        for (int i = 0; i < packageNames.Length; i++)
        {
            string packageName = packageNames[i];
            string message = "The " + packageName + " package is not installed. Please install it from the SDK Website";
            rules.Add(GenerateCheckExistPakageRule(packageName, message, helpLink));
        }

        return rules;
    }

    private static bool CheckExistPackage(string packageName)
    {
        {
            PackageInfo info = PackageInfo.FindForAssetPath("Packages/" + packageName);
            if (info == null)
            {
                return false;
            }
            return info.name.Contains(packageName);
        }
    }
    private static BuildValidationRule GenerateCheckExistPakageRule(string packageName, string message, string helpLink)
    {
        BuildValidationRule rule = new BuildValidationRule()
        {
            IsRuleEnabled = () => !CheckExistPackage(packageName),
            CheckPredicate = () => CheckExistPackage(packageName),
            Category = Category,
            //Message = "The com.qualcomm.snapdragon.spaces package is not installed. Please install it from the Package Manager.",
            Message = message,
            FixIt = () => Debug.LogError(message),
            FixItMessage = message,
            FixItAutomatic = false,
            Error = true,
            HelpLink = helpLink,
        };

        return rule;
    }


    public static void AddTargetDependentRules(List<BuildValidationRule> rules)
    {
        foreach (BuildValidationRule rule in rules)
        {
            if (!validationRules.Contains(rule))
            {
                validationRules.Add(rule);
            }
        }
        foreach (var item in BuildTargetGroups)
        {
            BuildValidator.AddRules(item, rules);  // Pass the new rules directly
        }
    }
}
#pragma warning restore IDE0051 // 사용되지 않는 private 멤버 제거
#endif