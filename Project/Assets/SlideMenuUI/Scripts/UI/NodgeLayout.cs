using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ノッジレイアウト
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
[DisallowMultipleComponent]
[ExecuteAlways]
public class NodgeLayout : UIBehaviour
{
    [System.Serializable]
    private enum LayoutType
    {
        Header,     // ヘッダー
        Footer,     // フッター
    }

    [SerializeField] private LayoutType type = LayoutType.Header;
    [SerializeField] private RectTransform nodge = null;

    private bool isChangedValidate_ = false;
    private bool isLock_ = false;

    /// <summary>
    /// ノッジを更新する
    /// </summary>
    public void UpdateNodge()
    {
        if (isLock_) { return;  }
        isLock_ = true;

        // VerticalLayoutGroup設定
        VerticalLayoutGroup layoutGroup = this.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childControlWidth = true;
        layoutGroup.childScaleWidth = false;
        layoutGroup.childForceExpandWidth = true;

        // ContentSizeFitter設定
        ContentSizeFitter sizeFitter = this.GetComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // RectTransform設定
        RectTransform selfRectTransform = this.GetComponent<RectTransform>();
        if (type == LayoutType.Header)
        {
            Vector2 pivot = new Vector2(0.0f, 1.0f);
            Vector2 anchorMin = new Vector2(0.0f, 1.0f);
            Vector2 anchorMax = new Vector2(1.0f, 1.0f);
            if (pivot != selfRectTransform.pivot || anchorMin != selfRectTransform.anchorMin || anchorMax != selfRectTransform.anchorMax)
            {
                selfRectTransform.pivot = pivot;
                selfRectTransform.anchorMin = anchorMin;
                selfRectTransform.anchorMax = anchorMax;
                selfRectTransform.offsetMin = Vector2.zero;
                selfRectTransform.offsetMax = Vector2.zero;
            }
        }
        else if (type == LayoutType.Footer)
        {
            Vector2 pivot = new Vector2(0.0f, 0.0f);
            Vector2 anchorMin = new Vector2(0.0f, 0.0f);
            Vector2 anchorMax = new Vector2(1.0f, 0.0f);
            if (pivot != selfRectTransform.pivot || anchorMin != selfRectTransform.anchorMin || anchorMax != selfRectTransform.anchorMax)
            {
                selfRectTransform.pivot = pivot;
                selfRectTransform.anchorMin = anchorMin;
                selfRectTransform.anchorMax = anchorMax;
                selfRectTransform.offsetMin = Vector2.zero;
                selfRectTransform.offsetMax = Vector2.zero;
            }
        }

        // ノッジ更新
        if (nodge != null)
        {
            float scale = 1.0f;
            CanvasScaler scaler = GetParentCanvasScaler(this.transform);
            var resolition = Screen.currentResolution;
            if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) { scale = scaler.referenceResolution.y / resolition.height; }
            Vector2 sizeDelta = nodge.sizeDelta;
            if (type == LayoutType.Header) { sizeDelta.y = (resolition.height - Screen.safeArea.yMax) * scale; }
            else if (type == LayoutType.Footer) { sizeDelta.y = Screen.safeArea.yMin * scale; }
            nodge.sizeDelta = sizeDelta;
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            sizeFitter.SetLayoutVertical();
        }

        isLock_ = false;
    }

    protected override void Awake()
    {
        UpdateNodge();
    }

    // Update is called once per frame
    void Update()
    {
        // インスペクター更新チェック
        if (isChangedValidate_)
        {
            isChangedValidate_ = false;
            UpdateNodge();
        }
    }

    protected override void OnEnable()
    {
        // インスペクター表示切替
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.hideFlags = HideFlags.NotEditable;
        ContentSizeFitter sizeFitter = this.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null) { sizeFitter.hideFlags = HideFlags.NotEditable; }
    }

    protected override void OnDisable()
    {
        // インスペクター表示切替
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.hideFlags = HideFlags.None;
        ContentSizeFitter sizeFitter = this.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null) { sizeFitter.hideFlags = HideFlags.None; }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        UpdateNodge();
    }

#if UNITY_EDITOR
    /// <summary>
    /// インスペクター変更検知
    /// </summary>
    protected override void OnValidate()
    {
        isChangedValidate_ = true;
    }
#endif

    /// <summary>
    /// 親キャンバスを取得する
    /// </summary>
    /// <returns></returns>
    private CanvasScaler GetParentCanvasScaler(Transform transform)
    {
        if (transform.parent == null) { return null; }

        CanvasScaler canvas = transform.parent.GetComponent<CanvasScaler>();
        if (canvas == null) { return GetParentCanvasScaler(transform.parent); }
        else { return canvas; }
    }
}
