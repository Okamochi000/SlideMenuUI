using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// コミットログ情報
/// </summary>
public class CommitLogText : MonoBehaviour
{
    private CommitLogInfo logInfo_ = null;
    private UnityAction<CommitLogInfo> callback_ = null;

    /// <summary>
    /// ログ設定
    /// </summary>
    /// <param name="logInfo"></param>
    public void SetLog(CommitLogInfo logInfo)
    {
        logInfo_ = logInfo;
        Text text = this.GetComponent<Text>();
        text.text = logInfo.message;
    }

    /// <summary>
    /// コールバック設定
    /// </summary>
    /// <param name="callback"></param>
    public void SetCallback(UnityAction<CommitLogInfo> callback)
    {
        callback_ = callback;
    }

    /// <summary>
    /// 押下判定
    /// </summary>
    public void OnClicked()
    {
        if (callback_ != null)
        {
            callback_(logInfo_);
        }
    }
}
