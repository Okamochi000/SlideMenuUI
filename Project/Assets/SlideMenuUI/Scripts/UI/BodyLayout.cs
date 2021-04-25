using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノッジレイアウト
/// </summary>
[ExecuteInEditMode]
public class BodyLayout : MonoBehaviour
{
    [SerializeField] private RectTransform header = null;
    [SerializeField] private RectTransform footer = null;
    [SerializeField] private bool isHeaderNodgeOnly = false;
    [SerializeField] private bool isFooterNodgeOnly = false;
    [SerializeField] private bool isPreview = false;

    private RectTransform selfRectTransform_ = null;
    private Vector2 screenSize_ = new Vector2();

    void Start()
    {
        screenSize_ = Vector2.zero;
        UpdateNodge();
        if (!Application.isEditor) { this.enabled = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPreview && !Application.isPlaying) { return; }

        if (screenSize_.x != Screen.currentResolution.width || screenSize_.y != Screen.currentResolution.height)
        {
            screenSize_.x = Screen.currentResolution.width;
            screenSize_.y = Screen.currentResolution.height;
            UpdateNodge();
        }
    }

    /// <summary>
    /// ノッジを更新する
    /// </summary>
    private void UpdateNodge()
    {
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }
        var resolition = Screen.currentResolution;
        var area = Screen.safeArea;
        selfRectTransform_.anchorMin = Vector2.zero;
        selfRectTransform_.anchorMax = Vector2.one;
        selfRectTransform_.offsetMin = Vector2.zero;
        selfRectTransform_.offsetMax = Vector2.zero;

        // ヘッダー設定
        if (isHeaderNodgeOnly)
        {
            selfRectTransform_.offsetMax = new Vector2(0, area.yMax - resolition.height);
        }
        else
        {
            if (header != null)
            {
                NodgeLayout nodgeLayout = header.GetComponent<NodgeLayout>();
                if (nodgeLayout != null) { nodgeLayout.UpdateNodge(); }
                selfRectTransform_.offsetMax = new Vector2(0, -header.sizeDelta.y);
            }
        }

        // フッター設定
        if (isFooterNodgeOnly)
        {
            selfRectTransform_.offsetMin = new Vector2(0, area.yMin);
        }
        else
        {
            if (footer != null)
            {
                NodgeLayout nodgeLayout = footer.GetComponent<NodgeLayout>();
                if (nodgeLayout != null) { nodgeLayout.UpdateNodge(); }
                selfRectTransform_.offsetMin = new Vector2(0, footer.sizeDelta.y);
            }
        }
    }
}
