using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ログ出力を監視する
/// </summary>
public class LogChecker : MonoBehaviourSingleton<LogChecker>
{
    /// <summary>
    /// ログ情報
    /// </summary>
    public class LogInfo
    {
        public string logText = "";
        public string stackTrace = "";
        public LogType logType = LogType.Log;

        public LogInfo() { }

        public LogInfo(string logText, string stackTrace, LogType logType)
        {
            this.logText = logText;
            this.stackTrace = stackTrace;
            this.logType = logType;
        }
    }

    public readonly int LogCountMax = 50;
    private readonly LogType[] ExceptLogTypes = { LogType.Warning };

    public LogInfo firstErrorLogInfo = null;
    public Queue<LogInfo> logQueue = new Queue<LogInfo>();
    public Action<LogInfo> addedCallBack = null;
    public Action clearCallBack = null;

    public int AllLogCount { get; private set; } = 0;
    public int[] LogCount { get; private set; } = new int[Enum.GetValues(typeof(LogType)).Length];

    private void OnEnable()
    {
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;
    }

    /// <summary>
    /// ログ取得
    /// </summary>
    /// <returns></returns>
    public LogInfo[] GetLogs()
    {
        return logQueue.ToArray();
    }

    /// <summary>
    /// ログ削除
    /// </summary>
    public void ClearLog()
    {
        firstErrorLogInfo = null;
        AllLogCount = 0;
        logQueue.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(LogType)).Length; i++) { LogCount[i] = 0; }
        if (clearCallBack != null) { clearCallBack(); }
    }

    private void OnLogMessage(string logText, string stackTrace, LogType type)
    {
        // 無視リストチェック
        foreach (LogType exceptType in ExceptLogTypes)
        {
            if (exceptType == type) { return; }
        }

        // ログ登録
        LogInfo logInfo = new LogInfo(logText, stackTrace, type);
        if (type != LogType.Log && type !!= LogType.Warning && firstErrorLogInfo == null) { firstErrorLogInfo = logInfo; }
        if (logQueue.Count == LogCountMax) { logQueue.Dequeue(); }
        logQueue.Enqueue(logInfo);

        // ログ数更新
        LogCount[(int)type]++;
        AllLogCount++;

        // コールバック呼び出し
        if (addedCallBack != null) { addedCallBack(logInfo); }
    }
}