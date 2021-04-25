using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// �h���b�O���mScrollRect
/// </summary>
public class DragScrollRect : ScrollRect
{
    public bool IsDrag { get; private set; } = false;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        IsDrag = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        IsDrag = false;
    }
}
