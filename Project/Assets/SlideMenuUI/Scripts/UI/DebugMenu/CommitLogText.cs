using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// �R�~�b�g���O���
/// </summary>
public class CommitLogText : MonoBehaviour
{
    private CommitLogInfo logInfo_ = null;
    private UnityAction<CommitLogInfo> callback_ = null;

    /// <summary>
    /// ���O�ݒ�
    /// </summary>
    /// <param name="logInfo"></param>
    public void SetLog(CommitLogInfo logInfo)
    {
        logInfo_ = logInfo;
        Text text = this.GetComponent<Text>();
        text.text = logInfo.message;
    }

    /// <summary>
    /// �R�[���o�b�N�ݒ�
    /// </summary>
    /// <param name="callback"></param>
    public void SetCallback(UnityAction<CommitLogInfo> callback)
    {
        callback_ = callback;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void OnClicked()
    {
        if (callback_ != null)
        {
            callback_(logInfo_);
        }
    }
}
