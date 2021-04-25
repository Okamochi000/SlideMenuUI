using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// �R�~�b�g���O����
/// </summary>
public class CommitLogGenerator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    /// �r���h�O����
    public void OnPreprocessBuild(BuildReport _report)
    {
        // �R�~�b�g���O�ݒ�
        CommitLogInfo.CreateFile();
    }

    /// �r���h�㏈��
    public void OnPostprocessBuild(BuildReport _report)
    {
        // �t�@�C���̒��g���폜����
        CommitLogInfo.ClearFile();
    }

    // �J�����[�h��
    private bool IsDevelopment(BuildReport report)
    {
        return (report.summary.options & BuildOptions.Development) != 0;
    }

    ///  ���s��
    public int callbackOrder { get { return 0; } }
}
