using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ログ表示
/// </summary>
public class LogViewer : MonoBehaviour
{
    public bool IsOpen { get; private set; } = false;

    [SerializeField] private Text allCountText = null;
    [SerializeField] private Text logCountText = null;
    [SerializeField] private Text errorCountText = null;
    [SerializeField] private Text logDetailText = null;
    [SerializeField] private Text errorDetailText = null;
    [SerializeField] private GameObject logBase = null;
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private ScrollRect logListScrollRect = null;
    [SerializeField] private ScrollRect logDetailScrollRect = null;
    [SerializeField] private ScrollRect errorDetailScrollRect = null;

    private Queue<GameObject> logQueue_ = new Queue<GameObject>();
    private HorizontalOrVerticalLayoutGroup layoutGroup_ = null;
    private ContentSizeFitter fitter_ = null;

    private void OnEnable()
    {
        if (!Application.isPlaying) { return; }
        LogChecker.Instance.addedCallBack += AddLog;
        if (!IsOpen) { Open(); }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying) { return; }
        if (LogChecker.Instance != null)
        {
            LogChecker.Instance.addedCallBack -= AddLog;
            if (IsOpen) { Close(); }
        }
    }

    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        // 表示切替
        IsOpen = true;
        this.gameObject.SetActive(true);

        // 初期設定
        ClearLogList();
        logBase.SetActive(false);
        scrollRect.verticalNormalizedPosition = 1.0f;
        if (layoutGroup_ == null) { layoutGroup_ = scrollRect.content.GetComponent<HorizontalOrVerticalLayoutGroup>(); }
        if (fitter_ == null) { fitter_ = scrollRect.content.GetComponent<ContentSizeFitter>(); }

        // ログ追加
        foreach (LogChecker.LogInfo logInfo in LogChecker.Instance.GetLogs()) { AddLog(logInfo); }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        // 表示切替
        IsOpen = false;
        this.gameObject.SetActive(false);

        // ログ削除
        ClearLogList();
    }

    /// <summary>
    /// ログ削除
    /// </summary>
    public void OnClearLog()
    {
        LogChecker.Instance.ClearLog();
        ClearLogList();
    }

    /// <summary>
    /// テストメッセージ送信
    /// </summary>
    public void OnSendTestMessage()
    {
        Debug.Log(String.Format("Test Log ({0})", DateTime.Now));
    }

    /// <summary>
    /// テストエラーメッセージ送信
    /// </summary>
    public void OnSendTestErrorMessage()
    {
        Debug.LogError(String.Format("Test ErrorLog ({0})", DateTime.Now));
    }

    /// <summary>
    /// ログ追加
    /// </summary>
    /// <param name="logInfo"></param>
    private void AddLog(LogChecker.LogInfo logInfo)
    {
        // ログ追加
        GameObject log = GameObject.Instantiate(logBase);
        log.transform.SetParent(logBase.transform.parent, false);
        log.transform.localScale = Vector3.one;
        log.SetActive(true);
        LogText logText = log.GetComponent<LogText>();
        logText.SetLog(logInfo);
        logText.SetCallback(OnViewDetail);
        logQueue_.Enqueue(log);
        logListScrollRect.enabled = true;

        // ログ削除
        while (logQueue_.Count > LogChecker.Instance.LogCountMax)
        {
            GameObject headLog = logQueue_.Dequeue();
            Destroy(headLog);
        }

        layoutGroup_.CalculateLayoutInputHorizontal();
        layoutGroup_.CalculateLayoutInputVertical();
        layoutGroup_.SetLayoutHorizontal();
        layoutGroup_.SetLayoutVertical();
        fitter_.SetLayoutHorizontal();
        fitter_.SetLayoutVertical();

        // エラーログ更新
        if (LogChecker.Instance.firstErrorLogInfo != null)
        {
            errorDetailText.text = GetDetailText(LogChecker.Instance.firstErrorLogInfo);
            errorDetailScrollRect.enabled = true;
        }
        else
        {
            errorDetailText.text = "";
        }

        // ログ数更新
        UpdateCountText();
    }

    /// <summary>
    /// ログリスト削除
    /// </summary>
    private void ClearLogList()
    {
        while (logQueue_.Count > 0)
        {
            GameObject log = logQueue_.Dequeue();
            Destroy(log);
        }

        // ログ数更新
        UpdateCountText();

        // 初期化
        errorDetailText.text = "";
        logDetailText.text = "";
        logListScrollRect.enabled = false;
        logDetailScrollRect.enabled = false;
        errorDetailScrollRect.enabled = false;
    }

    /// <summary>
    /// ログ数更新
    /// </summary>
    private void UpdateCountText()
    {
        int errorCount = LogChecker.Instance.LogCount[(int)LogType.Assert];
        errorCount += LogChecker.Instance.LogCount[(int)LogType.Error];
        errorCount += LogChecker.Instance.LogCount[(int)LogType.Exception];

        allCountText.text = LogChecker.Instance.AllLogCount.ToString();
        logCountText.text = LogChecker.Instance.LogCount[(int)LogType.Log].ToString();
        errorCountText.text = errorCount.ToString();
    }

    /// <summary>
    /// 詳細取得
    /// </summary>
    /// <returns></returns>
    private string GetDetailText(LogChecker.LogInfo logInfo)
    {
        return String.Format("[{0}] {1}\n{2}", logInfo.logType, logInfo.logText, logInfo.stackTrace);
    }

    /// <summary>
    /// 詳細表示
    /// </summary>
    private void OnViewDetail(LogChecker.LogInfo logInfo)
    {
        logDetailText.text = GetDetailText(logInfo);
        logDetailScrollRect.enabled = true;
    }
}