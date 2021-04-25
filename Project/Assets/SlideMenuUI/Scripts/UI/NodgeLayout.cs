using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �m�b�W���C�A�E�g
/// </summary>
[ExecuteInEditMode]
public class NodgeLayout : MonoBehaviour
{
    [System.Serializable]
    private enum LayoutType
    {
        Header,     // �w�b�_�[
        Footer,     // �t�b�^�[
    }

    [SerializeField] private LayoutType type = LayoutType.Header;
    [SerializeField] private RectTransform nodge = null;
    [SerializeField] private bool isPreview = false;

    private Vector2 screenSize_ = new Vector2();

    void Awake()
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
    /// �m�b�W���X�V����
    /// </summary>
    public void UpdateNodge()
    {
        if (nodge != null)
        {
            var resolition = Screen.currentResolution;
            Vector2 sizeDelta = nodge.sizeDelta;
            if (type == LayoutType.Header) { sizeDelta.y = resolition.height - Screen.safeArea.yMax; }
            else if (type == LayoutType.Footer) { sizeDelta.y = Screen.safeArea.yMin; }
            nodge.sizeDelta = sizeDelta;
            VerticalLayoutGroup layoutGroup = this.GetComponent<VerticalLayoutGroup>();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            this.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
    }
}