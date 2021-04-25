using System;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
#endif

/// <summary>
/// ÉrÉãÉhèÓïÒ
/// </summary>
[System.Serializable]
public class BuildInfo
{
    public string buildDate = "2000/01/01 00:00:00";
    public ColorSpace colorSpace = ColorSpace.Uninitialized;
    public bool isAutoGraphicsApi = false;
    public GraphicsDeviceType[] graphicsDeviceTypes = new GraphicsDeviceType[] { GraphicsDeviceType.Null };
    public string apiCompatibilityLevel = "";
    public bool[] isTierHDRs = new bool[Enum.GetValues(typeof(GraphicsTier)).Length];

    public static BuildInfo LoadEditorBuildInfo()
    {
        BuildInfo buildInfo = new BuildInfo();
#if UNITY_EDITOR
        buildInfo.buildDate = DateTime.Now.ToString();
        buildInfo.colorSpace = PlayerSettings.colorSpace;
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor: buildTarget = BuildTarget.StandaloneWindows64; break;
            case RuntimePlatform.LinuxEditor: buildTarget = BuildTarget.StandaloneLinux64; break;
            case RuntimePlatform.OSXEditor: buildTarget = BuildTarget.StandaloneOSX; break;
        }
        buildInfo.isAutoGraphicsApi = PlayerSettings.GetUseDefaultGraphicsAPIs(buildTarget);
        buildInfo.graphicsDeviceTypes = PlayerSettings.GetGraphicsAPIs(buildTarget);
        buildInfo.apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone).ToString();
        foreach (GraphicsTier type in Enum.GetValues(typeof(GraphicsTier))) { buildInfo.isTierHDRs[(int)type] = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, type).hdr; }
#endif
        return buildInfo;
    }
}