using UnityEngine;

/// <summary>
/// デバッグメニュー
/// </summary>
public class DebugMenu : MonoBehaviour
{
    [SerializeField] private SlideMenu slideMenu = null;
    [SerializeField] private BuildInfoViewer buildInfoViewer = null;
    [SerializeField] private LogViewer logViewer = null;

    public void Start()
    {
        slideMenu.gameObject.SetActive(true);
        slideMenu.ResetPosition();
    }

    /// <summary>
    /// ログビューア表示
    /// </summary>
    public void OnOpenLogViewer()
    {
        logViewer.Open();
        buildInfoViewer.Close();
        slideMenu.Close();
    }

    /// <summary>
    /// ビルド情報表示
    /// </summary>
    public void OnOpenBuildInfoViewer()
    {
        buildInfoViewer.Open();
        logViewer.Close();
        slideMenu.Close();
    }
}
