using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ビルド情報表示
/// </summary>
public class BuildInfoViewer : MonoBehaviour
{
    public bool IsOpen { get; private set; } = false;

    [SerializeField] private Text versionText = null;
    [SerializeField] private Text buildDateText = null;
    [SerializeField] private Text colorSpaceText = null;
    [SerializeField] private Text isAutoGraphicsApiText = null;
    [SerializeField] private Text graphicsDeviceTypeText = null;
    [SerializeField] private Text apiCompatibilityLevelText = null;
    [SerializeField] private Text qualityText = null;
    [SerializeField] private Text graphicTierTypeText = null;
    [SerializeField] private Text graphicTierHDRText = null;
    [SerializeField] private GameObject commitBase = null;
    [SerializeField] private Text commitDetailText = null;
    [SerializeField] private ScrollRect commitListScrollRect = null;
    [SerializeField] private ScrollRect commitDetailScrollRect = null;

    private List<GameObject> commitObjList_ = new List<GameObject>();
    private List<CommitLogInfo> commintInfoList_ = new List<CommitLogInfo>();

    private void OnEnable()
    {
        if (!Application.isPlaying) { return; }
        if (!IsOpen) { Open(); }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying) { return; }
        if (IsOpen) { Close(); }
    }

    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        // 表示切替
        IsOpen = true;
        this.gameObject.SetActive(true);

        // ビルド情報取得
        BuildInfo buildInfo = null;
        if (Application.isEditor)
        {
            buildInfo = BuildInfo.LoadEditorBuildInfo();
        }
        else
        {
            TextAsset textAsset = Resources.Load<TextAsset>("build_info");
            buildInfo = JsonUtility.FromJson<BuildInfo>(textAsset.text);
        }

        // ビルド情報設定
        versionText.text = Application.version;
        buildDateText.text = buildInfo.buildDate;
        colorSpaceText.text = buildInfo.colorSpace.ToString();
        isAutoGraphicsApiText.text = buildInfo.isAutoGraphicsApi.ToString();
        graphicsDeviceTypeText.text = buildInfo.graphicsDeviceTypes[0].ToString();
        apiCompatibilityLevelText.text = buildInfo.apiCompatibilityLevel;
        qualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        graphicTierTypeText.text = Graphics.activeTier.ToString();
        graphicTierHDRText.text = buildInfo.isTierHDRs[(int)Graphics.activeTier].ToString();

        // コミット履歴設定
        commitBase.SetActive(false);
        commitDetailText.text = "";
        commitDetailScrollRect.enabled = false;
        foreach (GameObject obj in commitObjList_) { Destroy(obj); }
        commitObjList_.Clear();
        commintInfoList_.Clear();
        commintInfoList_ = new List<CommitLogInfo>(CommitLogInfo.LoadCommitLog(Application.isEditor));
        commintInfoList_.Reverse();
        foreach (CommitLogInfo commitLogInfo in commintInfoList_)
        {
            // ログ追加
            GameObject log = GameObject.Instantiate(commitBase);
            log.transform.SetParent(commitBase.transform.parent, true);
            log.SetActive(true);
            CommitLogText logText = log.GetComponent<CommitLogText>();
            logText.SetLog(commitLogInfo);
            logText.SetCallback(OnViewDetail);
            commitObjList_.Add(log);
        }
        if (commitObjList_.Count > 0) { commitListScrollRect.enabled = true; }
        else { commitListScrollRect.enabled = false; }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        // 表示切替
        IsOpen = false;
        this.gameObject.SetActive(false);

        // コミット履歴削除
        foreach (GameObject obj in commitObjList_) { Destroy(obj); }
        commitObjList_.Clear();
        commintInfoList_.Clear();
        commitDetailText.text = "";
        commitDetailScrollRect.enabled = false;
    }

    /// <summary>
    /// 詳細表示
    /// </summary>
    private void OnViewDetail(CommitLogInfo logInfo)
    {
        commitDetailText.text = String.Format("[{0}]\n[{1}]\n{2}", logInfo.commitType, logInfo.hash, logInfo.message);
        commitDetailScrollRect.enabled = true;
    }
}
