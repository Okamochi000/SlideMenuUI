using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using UnityEditor.Rendering;

/// <summary>
/// ビルド情報生成
/// </summary>
public class BuildInfoGenerator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    private readonly string BuildInfoPath = String.Format("{0}/SlideMenuUI/Resources/{1}.json", Application.dataPath, "build_info");

    /// ビルド前処理
    public void OnPreprocessBuild(BuildReport _report)
    {
        // JSON形式にシリアライズ
        BuildInfo buildInfo = new BuildInfo();
        buildInfo.buildDate = DateTime.Now.ToString();
        buildInfo.colorSpace = PlayerSettings.colorSpace;
        buildInfo.isAutoGraphicsApi = PlayerSettings.GetUseDefaultGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget);
        buildInfo.graphicsDeviceTypes = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget);
        buildInfo.apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(ToBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)).ToString();
        foreach (GraphicsTier type in Enum.GetValues(typeof(GraphicsTier))) { buildInfo.isTierHDRs[(int)type] = EditorGraphicsSettings.GetTierSettings(ToBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), type).hdr; }
        var json = JsonUtility.ToJson(buildInfo, false);

        // JSONデータをファイルに保存
        File.WriteAllText(BuildInfoPath, json);
    }

    /// ビルド後処理
    public void OnPostprocessBuild(BuildReport _report)
    {
        // ファイルの中身を削除する
        File.WriteAllText(BuildInfoPath, "");
    }

    // 開発モードか
    private bool IsDevelopment(BuildReport report)
    {
        return (report.summary.options & BuildOptions.Development) != 0;
    }

    ///  実行順
    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// BuildTargetからBuildTargetGroupへの変換
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
