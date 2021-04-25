using UnityEngine;

/// <summary>
/// �f�o�b�O���j���[
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
    /// ���O�r���[�A�\��
    /// </summary>
    public void OnOpenLogViewer()
    {
        logViewer.Open();
        buildInfoViewer.Close();
        slideMenu.Close();
    }

    /// <summary>
    /// �r���h���\��
    /// </summary>
    public void OnOpenBuildInfoViewer()
    {
        buildInfoViewer.Open();
        logViewer.Close();
        slideMenu.Close();
    }
}
