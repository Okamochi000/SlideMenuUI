using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コミット上夫雄
/// </summary>
[System.Serializable]
public class CommitLogInfo
{
    [System.Serializable]
    public class JsonHelper
    {
        public CommitLogInfo[] logInfos = null;
    }

    private static readonly string ResoucesPath = "commit_log";
    private static readonly int CommitLogBackCount = 2;
    private static readonly int CommitCount = 50;
    private static string CommitLogPath { get { return String.Format("{0}/SlideMenuUI/Resources/{1}.json", Application.dataPath, ResoucesPath); } }

    public string hash = "";
    public string commitType = "";
    public string message = "";

    /// <summary>
    /// ファイルを作成する
    /// </summary>
    public static void CreateFile()
    {
        // コミットログ設定
        string commitPath = GetCommitLogPath(CommitLogBackCount);
        if (commitPath != "")
        {
            List<CommitLogInfo> infoList = new List<CommitLogInfo>();
            string[] lines = File.ReadAllLines(commitPath);
            int startIndex = Math.Max(0, lines.Length - CommitCount);
            for (int i = startIndex; i < lines.Length; i++) { infoList.Add(CreateCommitLogInfo(lines[i])); }
            JsonHelper helper = new JsonHelper();
            helper.logInfos = infoList.ToArray();
            var json = JsonUtility.ToJson(helper, false);
            File.WriteAllText(CommitLogPath, json);
        }
    }

    /// <summary>
    /// ファイルの中身を削除する
    /// </summary>
    public static void ClearFile()
    {
        // コミットログ設定
        if (File.Exists(CommitLogPath)) { File.WriteAllText(CommitLogPath, ""); }
    }

    /// <summary>
    /// ファイルの読み込み
    /// </summary>
    /// <param name="isEditor"></param>
    /// <returns></returns>
    public static CommitLogInfo[] LoadCommitLog(bool isEditor)
    {
        if (isEditor)
        {
            List<CommitLogInfo> infoList = new List<CommitLogInfo>();
            string commitPath = GetCommitLogPath(CommitLogBackCount);
            if (commitPath != "")
            {
                string[] lines = File.ReadAllLines(commitPath);
                int startIndex = Math.Max(0, lines.Length - CommitCount);
                for (int i = startIndex; i < lines.Length; i++) { infoList.Add(CreateCommitLogInfo(lines[i])); }
            }
            return infoList.ToArray();
        }
        else
        {
            UnityEngine.Object resources = Resources.Load(ResoucesPath);
            if (resources != null)
            {
                TextAsset textAsset = Resources.Load<TextAsset>(ResoucesPath);
                if (textAsset.text != "")
                {
                    JsonHelper helper = JsonUtility.FromJson<JsonHelper>(textAsset.text);
                    return helper.logInfos;
                }
            }
            return new CommitLogInfo[0];
        }
    }

    /// <summary>
    /// ログ1行をCommitLogInfoに変換
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static CommitLogInfo CreateCommitLogInfo(string line)
    {
        CommitLogInfo commitLogInfo = new CommitLogInfo();
        string[] splits = line.Split(' ');
        int removeCount = 0;
        for (int i = 0; i < 6; i++) { removeCount += splits[i].Length + 1; }
        string commitType = splits[5].Split('\t')[1];
        commitLogInfo.hash = splits[1];
        commitLogInfo.commitType = commitType.Remove(commitType.Length - 1);
        commitLogInfo.message = line.Remove(0, removeCount);

        return commitLogInfo;
    }

    /// <summary>
    /// コミットログのパスを取得する
    /// </summary>
    /// <param name="backCount"></param>
    /// <returns></returns>
    private static string GetCommitLogPath(int backCount)
    {
        string path = Application.dataPath;
        string[] splits = path.Split('/');
        if (splits.Length <= CommitLogBackCount) { return ""; }

        string removePath = "";
        for (int i = 0; i < CommitLogBackCount; i++)
        {
            int num = splits.Length - CommitLogBackCount + i;
            removePath += String.Format("/{0}", splits[num]);
        }
        path = path.Remove(path.LastIndexOf(removePath));
        path += "/.git/logs/HEAD";
        if (File.Exists(path)) { return path; }

        return "";
    }
}
