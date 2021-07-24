using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �Z�[�t�G���A�������C�A�E�g
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class IgnoneSafeAreaLayout : UIBehaviour
{
    [SerializeField] private bool isTopSafeArea = false;
    [SerializeField] private bool isBottomSafeArea = false;
    [SerializeField] private bool isLeftSafeArea = false;
    [SerializeField] private bool isRightSafeArea = false;

    private RectTransform canvasRectTransform_ = null;
    private RectTransform selfRectTransform_ = null;
    private bool isLock_ = false;
    private bool isSafeAreaLayout_ = false;
    private bool isChangedValidate_ = false;

    /// <summary>
    /// �m�b�W���X�V����
    /// </summary>
    public void UpdateRectTransform()
    {
        if (isLock_) { return; }
        if (canvasRectTransform_ == null) { return; }
        if (selfRectTransform_ == null) { return; }

        isLock_ = true;
        isSafeAreaLayout_ = false;
        isChangedValidate_ = false;

        // �Z�[�t�G���A�𖳎������T�C�Y�ɒ�������
        Vector2 sizeDelta = canvasRectTransform_.sizeDelta;
        selfRectTransform_.pivot = new Vector2(0.5f, 0.5f);
        selfRectTransform_.anchorMin = Vector2.zero;
        selfRectTransform_.anchorMax = Vector2.one;
        selfRectTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasRectTransform_.sizeDelta.x);
        selfRectTransform_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasRectTransform_.sizeDelta.y);
        selfRectTransform_.position = canvasRectTransform_.position;

        // �Z�[�t�G���A���Ɏ��߂�
        Vector2 offsetMin = selfRectTransform_.offsetMin;
        Vector2 offsetMax = selfRectTransform_.offsetMax;
        Vector2 outsideOffsetMin = GetOutsideOffsetMin();
        Vector2 outsideOffsetMax = GetOutsideOffsetMax();
        if (isTopSafeArea) { offsetMax.y += outsideOffsetMax.y; }
        if (isBottomSafeArea) { offsetMin.y += outsideOffsetMin.y; }
        if (isLeftSafeArea) { offsetMax.x += outsideOffsetMax.x; }
        if (isRightSafeArea) { offsetMin.x += outsideOffsetMin.x; }
        selfRectTransform_.offsetMin = offsetMin;
        selfRectTransform_.offsetMax = offsetMax;

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
        CanvasScaler canvasScaler = GetParentCanvasScaler(this.transform);
        if (canvasScaler != null) { canvasRectTransform_ = canvasScaler.GetComponent<RectTransform>(); }
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }
        UpdateRectTransform();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (!isLock_ && !isSafeAreaLayout_)
        {
            SafeAreaLayout safeAreaLayout = GetSafeAreaLayout(this.transform);
            if (safeAreaLayout != null && safeAreaLayout.IsUpdating)
            {
                isSafeAreaLayout_ = true;
                safeAreaLayout.tempUpdatedCallback += UpdateRectTransform;
            }
            else
            {
                CanvasScaler canvasScaler = GetParentCanvasScaler(this.transform);
                if (canvasScaler != null) { canvasRectTransform_ = canvasScaler.GetComponent<RectTransform>(); }
                UpdateRectTransform();
            }
        }
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
    /// �Z�[�t�G���A�O�擾(�{�g���A���t�g)
    /// </summary>
    /// <returns></returns>
    private Vector2 GetOutsideOffsetMin()
    {
        var resolition = Screen.currentResolution;
        var area = Screen.safeArea;
        float scale = 1.0f;
        CanvasScaler scaler = GetParentCanvasScaler(this.transform);
        if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) { scale = scaler.referenceResolution.y / resolition.height; }

        Vector2 offsetMin = Vector2.zero;
        offsetMin.y = area.yMin * scale;
        offsetMin.x = area.xMin * scale;

        return offsetMin;
    }

    /// <summary>
    /// ���E�Z�[�t�G���A�O�擾(�g�b�v�A���C�g)
    /// </summary>
    /// <returns></returns>
    private Vector2 GetOutsideOffsetMax()
    {
        var resolition = Screen.currentResolution;
        var area = Screen.safeArea;
        float scale = 1.0f;
        CanvasScaler scaler = GetParentCanvasScaler(this.transform);
        if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize) { scale = scaler.referenceResolution.y / resolition.height; }

        Vector2 offsetMax = Vector2.zero;
        offsetMax.y = (area.yMax - resolition.height) * scale;
        offsetMax.x = (area.xMax - resolition.width) * scale;

        return offsetMax;
    }

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

    /// <summary>
    /// �eBodyLayout���擾����
    /// </summary>
    /// <returns></returns>
    private SafeAreaLayout GetSafeAreaLayout(Transform transform)
    {
        if (transform.parent == null) { return null; }

        SafeAreaLayout safeAreaLayout = transform.parent.GetComponent<SafeAreaLayout>();
        if (safeAreaLayout == null) { return GetSafeAreaLayout(transform.parent); }
        else { return safeAreaLayout; }
    }
}
