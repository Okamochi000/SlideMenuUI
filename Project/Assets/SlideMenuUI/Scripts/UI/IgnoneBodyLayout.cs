using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ノッジ無視レイアウト
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class IgnoneBodyLayout : UIBehaviour
{
    private RectTransform canvasRectTransform_ = null;
    private RectTransform selfRectTransform_ = null;
    private bool isLock_ = false;
    private bool isBodyLayout_ = false;
    private bool isChangedValidate_ = false;

    /// <summary>
    /// ノッジを更新する
    /// </summary>
    public void UpdateRectTransform()
    {
        if (isLock_) { return; }
        if (canvasRectTransform_ == null) { return; }
        if (selfRectTransform_ == null) { return; }

        isLock_ = true;
        isBodyLayout_ = false;
        isChangedValidate_ = false;

        Vector2 sizeDelta = canvasRectTransform_.sizeDelta;
        selfRectTransform_.pivot = new Vector2(0.5f, 0.5f);
        selfRectTransform_.anchorMin = Vector2.zero;
        selfRectTransform_.anchorMax = Vector2.one;
        selfRectTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasRectTransform_.sizeDelta.x);
        selfRectTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasRectTransform_.sizeDelta.y);
        selfRectTransform_.position = canvasRectTransform_.position;

        isLock_ = false;
    }

    protected override void Awake()
    {
        UpdateRectTransform();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (isChangedValidate_)
        {
            UpdateRectTransform();
        }
    }

    protected override void OnEnable()
    {
        // インスペクター表示切替
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.hideFlags = HideFlags.NotEditable;
        Canvas canvas = GetParentCanvas(this.transform);
        if (canvas != null) { canvasRectTransform_ = canvas.GetComponent<RectTransform>(); }
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }
        UpdateRectTransform();
    }

    protected override void OnDisable()
    {
        // インスペクター表示切替
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.hideFlags = HideFlags.None;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (!isLock_ && !isBodyLayout_)
        {
            BodyLayout bodyLayout = GetBodyLayoutCanvas(this.transform);
            if (bodyLayout != null && bodyLayout.IsUpdating)
            {
                isBodyLayout_ = true;
                bodyLayout.tempUpdatedCallback += UpdateRectTransform;
            }
            else
            {
                Canvas canvas = GetParentCanvas(this.transform);
                if (canvas != null) { canvasRectTransform_ = canvas.GetComponent<RectTransform>(); }
                UpdateRectTransform();
            }
        }
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
    private Canvas GetParentCanvas(Transform transform)
    {
        if (transform.parent == null) { return null; }

        Canvas canvas = transform.parent.GetComponent<Canvas>();
        if (canvas == null) { return GetParentCanvas(transform.parent); }
        else { return canvas; }
    }

    /// <summary>
    /// 親BodyLayoutを取得する
    /// </summary>
    /// <returns></returns>
    private BodyLayout GetBodyLayoutCanvas(Transform transform)
    {
        if (transform.parent == null) { return null; }

        BodyLayout bodyLayout = transform.parent.GetComponent<BodyLayout>();
        if (bodyLayout == null) { return GetBodyLayoutCanvas(transform.parent); }
        else { return bodyLayout; }
    }
}
