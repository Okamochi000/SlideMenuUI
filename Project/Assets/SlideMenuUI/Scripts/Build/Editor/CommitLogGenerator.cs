using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// コミットログ生成
/// </summary>
public class CommitLogGenerator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    /// ビルド前処理
    public void OnPreprocessBuild(BuildReport _report)
    {
        // コミットログ設定
        CommitLogInfo.CreateFile();
    }

    /// ビルド後処理
    public void OnPostprocessBuild(BuildReport _report)
    {
        // ファイルの中身を削除する
        CommitLogInfo.ClearFile();
    }

    // 開発モードか
    private bool IsDevelopment(BuildReport report)
    {
        return (report.summary.options & BuildOptions.Development) != 0;
    }

    ///  実行順
    public int callbackOrder { get { return 0; } }
}
