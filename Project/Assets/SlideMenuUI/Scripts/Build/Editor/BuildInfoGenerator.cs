using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using UnityEditor.Rendering;

/// <summary>
/// �r���h��񐶐�
/// </summary>
public class BuildInfoGenerator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    private readonly string BuildInfoPath = String.Format("{0}/SlideMenuUI/Resources/{1}.json", Application.dataPath, "build_info");

    /// �r���h�O����
    public void OnPreprocessBuild(BuildReport _report)
    {
        // JSON�`���ɃV���A���C�Y
        BuildInfo buildInfo = new BuildInfo();
        buildInfo.buildDate = DateTime.Now.ToString();
        buildInfo.colorSpace = PlayerSettings.colorSpace;
        buildInfo.isAutoGraphicsApi = PlayerSettings.GetUseDefaultGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget);
        buildInfo.graphicsDeviceTypes = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget);
        buildInfo.apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(ToBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)).ToString();
        foreach (GraphicsTier type in Enum.GetValues(typeof(GraphicsTier))) { buildInfo.isTierHDRs[(int)type] = EditorGraphicsSettings.GetTierSettings(ToBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), type).hdr; }
        var json = JsonUtility.ToJson(buildInfo, false);

        // JSON�f�[�^���t�@�C���ɕۑ�
        File.WriteAllText(BuildInfoPath, json);
    }

    /// �r���h�㏈��
    public void OnPostprocessBuild(BuildReport _report)
    {
        // �t�@�C���̒��g���폜����
        File.WriteAllText(BuildInfoPath, "");
    }

    // �J�����[�h��
    private bool IsDevelopment(BuildReport report)
    {
        return (report.summary.options & BuildOptions.Development) != 0;
    }

    ///  ���s��
    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// BuildTarget����BuildTargetGroup�ւ̕ϊ�
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    private BuildTargetGroup ToBuildTargetGroup(BuildTarget self)
    {
        switch (self)
        {
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return BuildTargetGroup.Standalone;
            case BuildTarget.iOS: return BuildTargetGroup.iOS;
            case BuildTarget.Android: return BuildTargetGroup.Android;
            case BuildTarget.WebGL: return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
            case BuildTarget.PS4: return BuildTargetGroup.PS4;
            case BuildTarget.XboxOne: return BuildTargetGroup.XboxOne;
            case BuildTarget.tvOS: return BuildTargetGroup.tvOS;
            case BuildTarget.Switch: return BuildTargetGroup.Switch;
            case BuildTarget.Lumin: return BuildTargetGroup.Lumin;
            case BuildTarget.Stadia: return BuildTargetGroup.Stadia;
        }

        return BuildTargetGroup.Standalone;
    }
}
