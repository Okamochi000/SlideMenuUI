using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �Z�[�t�G���A�O���C�A�E�g
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public abstract class OutsideLayoutBase : UIBehaviour
{
    [SerializeField] protected RectTransform outside = null;

    private bool isChangedValidate_ = false;
    private bool isLock_ = false;
    private RectTransform selfRectTransform_ = null;

    /// <summary>
    /// ���C�A�E�g���X�V����
    /// </summary>
    public void UpdateLayout()
    {
        if (isLock_) { return; }

        isLock_ = true;
        isChangedValidate_ = false;

        OnUpdateLayout();

        isLock_ = false;
    }

    /// <summary>
    /// RectTransform�擾
    /// </summary>
    /// <returns></returns>
    public RectTransform GetRectTransform()
    {
        if (selfRectTransform_ == null) { selfRectTransform_ = this.GetComponent<RectTransform>(); }
        return selfRectTransform_;
    }

    protected override void Awake()
    {
        base.Awake();
        UpdateLayout();
    }

    // Update is called once per frame
    protected void Update()
    {
        // �C���X�y�N�^�[�X�V�`�F�b�N
        if (isChangedValidate_)
        {
            UpdateLayout();
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateLayout();
    }

#if UNITY_EDITOR
    /// <summary>
    /// �C���X�y�N�^�[�ύX���m
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
        isChangedValidate_ = true;
    }
#endif

    /// <summary>
    /// ���C�A�E�g�X�V
    /// </summary>
    protected abstract void OnUpdateLayout();

    /// <summary>
    /// �Z�[�t�G���A�O�擾(�{�g���A���t�g)
    /// </summary>
    /// <returns></returns>
    protected Vector2 GetOutsideOffsetMin()
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
    protected Vector2 GetOutsideOffsetMax()
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
}
