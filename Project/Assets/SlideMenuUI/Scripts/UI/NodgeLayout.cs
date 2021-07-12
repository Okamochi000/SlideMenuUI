using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �m�b�W���C�A�E�g
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
        Header,     // �w�b�_�[
        Footer,     // �t�b�^�[
    }

    [SerializeField] private LayoutType type = LayoutType.Header;
    [SerializeField] private RectTransform nodge = null;

    private bool isChangedValidate_ = false;
    private bool isLock_ = false;

    /// <summary>
    /// �m�b�W���X�V����
    /// </summary>
    public void UpdateNodge()
    {
        if (isLock_) { return;  }
        isLock_ = true;

        // VerticalLayoutGroup�ݒ�
        VerticalLayoutGroup layoutGroup = this.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childControlWidth = true;
        layoutGroup.childScaleWidth = false;
        layoutGroup.childForceExpandWidth = true;

        // ContentSizeFitter�ݒ�
        ContentSizeFitter sizeFitter = this.GetComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // RectTransform�ݒ�
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

        // �m�b�W�X�V
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
        // �C���X�y�N�^�[�X�V�`�F�b�N
        if (isChangedValidate_)
        {
            isChangedValidate_ = false;
            UpdateNodge();
        }
    }

    protected override void OnEnable()
    {
        // �C���X�y�N�^�[�\���ؑ�
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.hideFlags = HideFlags.NotEditable;
        ContentSizeFitter sizeFitter = this.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null) { sizeFitter.hideFlags = HideFlags.NotEditable; }
    }

    protected override void OnDisable()
    {
        // �C���X�y�N�^�[�\���ؑ�
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
    /// �C���X�y�N�^�[�ύX���m
    /// </summary>
    protected override void OnValidate()
    {
        isChangedValidate_ = true;
    }
#endif

    /// <summary>
    /// �e�L�����o�X���擾����
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
