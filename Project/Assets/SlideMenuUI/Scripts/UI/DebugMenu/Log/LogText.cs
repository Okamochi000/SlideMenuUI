using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// ログ情報
/// </summary>
public class LogText : MonoBehaviour
{
    [SerializeField] private Color errorColor = Color.red;

    private LogChecker.LogInfo logInfo_ = null;
    private UnityAction<LogChecker.LogInfo> callback_ = null;

    /// <summary>
    /// ログ設定
    /// </summary>
    /// <param name="logInfo"></param>
    public void SetLog(LogChecker.LogInfo logInfo)
    {
        logInfo_ = logInfo;
        Text text = this.GetComponent<Text>();
        text.text = String.Format("[{0}] {1}", logInfo.logType, logInfo.logText);
        if (logInfo.logType != LogType.Log && logInfo.logType != LogType.Warning) { text.color = errorColor; }
    }

    /// <summary>
    /// コールバック設定
    /// </summary>
    /// <param name="callback"></param>
    public void SetCallback(UnityAction<LogChecker.LogInfo> callback)
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
