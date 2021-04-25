using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlideMenu : MonoBehaviour
{
    private enum AutoSlideType
    {
        None,
        Open,
        Close
    }

    [SerializeField] private DragScrollRect scrollRect = null;
    [SerializeField] private Image clickBlocker = null;
    [SerializeField] private Image grapImage = null;
    [SerializeField] [Min(0)] private float fadeAlpha = 0.5f;
    [SerializeField] [Min(0.1f)] private float openAndCloseTime = 0.5f;
    [SerializeField] private bool isLeft = false;

    private bool isPrevDrag_ = false;
    private bool isOpenDrag_ = false;
    private bool isOpenDragMove_ = false;
    private float prevNormPos_= 0.0f;
    private float playTime_ = 0.0f;
    private Vector2 prevTouchPos_ = Vector2.zero;
    private Vector2 startTouchPos_ = Vector2.zero;
    private Vector2 startOffsetMin_ = Vector2.zero;
    private AutoSlideType autoType_ = AutoSlideType.None;

    void Awake()
    {
        Vector2 anchorMin = scrollRect.content.anchorMin;
        Vector2 anchorMax = scrollRect.content.anchorMax;
        if (isLeft) { anchorMin.x = -1.0f; }
        else { anchorMin.x = -2.0f; }
        anchorMax.x = anchorMin.x + 2.0f;
        scrollRect.content.anchorMin = anchorMin;
        scrollRect.content.anchorMax = anchorMax;

        if (!isLeft)
        {
            RectTransform rectTransform = scrollRect.GetComponent<RectTransform>();
            anchorMin = rectTransform.anchorMin;
            anchorMax = rectTransform.anchorMax;
            anchorMax.x = 2.0f - anchorMin.x;
            anchorMin.x = 1.0f;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // 開いているときに画面をタップされたか
        if (!isOpenDrag_ && IsScreenTouch(false))
        {
            if (scrollRect.horizontalNormalizedPosition > (GetOpenNormPos() - 0.002f) && scrollRect.horizontalNormalizedPosition < (GetOpenNormPos() + 0.002f))
            {
                isOpenDrag_ = true;
                prevTouchPos_ = GetTouchPosition();
                startTouchPos_ = prevTouchPos_;
                startOffsetMin_ = scrollRect.content.offsetMin;
            }
        }

        // 開いているときスライド
        if (isOpenDrag_)
        {
            if (IsScreenTouch(true))
            {
                Vector2 offsetMin = scrollRect.content.offsetMin;
                offsetMin.x += (GetTouchPosition() - prevTouchPos_).x;
                if (isLeft) { offsetMin.x = Mathf.Min(offsetMin.x, startOffsetMin_.x); }
                else { offsetMin.x = Mathf.Max(offsetMin.x, startOffsetMin_.x); }
                scrollRect.content.offsetMin = offsetMin;
                scrollRect.content.offsetMax = offsetMin;
                prevTouchPos_ = GetTouchPosition();
                if (!isOpenDragMove_)
                {
                    if (Mathf.Abs(startTouchPos_.x - GetTouchPosition().x) > 10.0f || Mathf.Abs(startTouchPos_.y - GetTouchPosition().y) > 10.0f)
                    {
                        isOpenDragMove_ = true;
                    }
                }
            }
            else
            {
                isOpenDrag_ = false;
                isOpenDragMove_ = false;
                grapImage.raycastTarget = true;
                playTime_ = 0.0f;
                prevNormPos_ = scrollRect.horizontalNormalizedPosition;
                EndedTouch();
            }
        }

        // ドラッグ開始されたら自動移動停止
        if (!isPrevDrag_ && scrollRect.IsDrag) { autoType_ = AutoSlideType.None; }

        // ドラッグを離したら自動で移動する
        if (isPrevDrag_ && !scrollRect.IsDrag)
        {
            if (autoType_ == AutoSlideType.None) { EndedTouch(); }
        }
        isPrevDrag_ = scrollRect.IsDrag;

        // スクロール値の変更がない場合は無効
        if (autoType_ == AutoSlideType.None && prevNormPos_ == scrollRect.horizontalNormalizedPosition)
        {
            if (prevNormPos_ == 0.0f || prevNormPos_ == 1.0f) { return; }
        }

        // 自動開閉
        if (!isOpenDrag_)
        {
            if (autoType_ == AutoSlideType.Open)
            {
                playTime_ += Time.deltaTime;
                playTime_ = Mathf.Min(playTime_, openAndCloseTime);
                float lerpT = Easing(playTime_, openAndCloseTime);
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, GetOpenNormPos(), lerpT);
                if (lerpT == 1.0f)
                {
                    autoType_ = AutoSlideType.None;
                }
            }
            else if (autoType_ == AutoSlideType.Close)
            {
                playTime_ += Time.deltaTime;
                playTime_ = Mathf.Min(playTime_, openAndCloseTime);
                float lerpT = Easing(playTime_, openAndCloseTime);
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, GetCloseNormPos(), lerpT);
                if (lerpT == 1.0f) { autoType_ = AutoSlideType.None; }
            }
        }

        // 背景更新
        if ((isLeft && scrollRect.horizontalNormalizedPosition > 0.998f) || (!isLeft && scrollRect.horizontalNormalizedPosition < 0.002f))
        {
            if (!scrollRect.IsDrag)
            {
                ResetPosition();
                prevNormPos_ = scrollRect.horizontalNormalizedPosition;
            }
        }
        else
        {
            clickBlocker.gameObject.SetActive(true);
            Color color = clickBlocker.color;
            if (isLeft) { color.a = (1.0f - scrollRect.horizontalNormalizedPosition) * fadeAlpha; }
            else { color.a = scrollRect.horizontalNormalizedPosition * fadeAlpha; }
            clickBlocker.color = color;
            prevNormPos_ = scrollRect.horizontalNormalizedPosition;
        }
    }
    
    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        if (autoType_ != AutoSlideType.Open && scrollRect.horizontalNormalizedPosition != GetOpenNormPos())
        {
            if (isLeft) { playTime_ = (1.0f - scrollRect.horizontalNormalizedPosition) * openAndCloseTime; }
            else { playTime_ = scrollRect.horizontalNormalizedPosition * openAndCloseTime; }
            playTime_ = 0.0f;
            autoType_ = AutoSlideType.Open;
            isOpenDrag_ = false;
            grapImage.raycastTarget = false;
        }
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        if (autoType_ != AutoSlideType.Close && scrollRect.horizontalNormalizedPosition != GetCloseNormPos())
        {
            if (isLeft) { playTime_ = scrollRect.horizontalNormalizedPosition * openAndCloseTime; }
            else { playTime_ = (1.0f - scrollRect.horizontalNormalizedPosition) * openAndCloseTime; }
            playTime_ = 0.0f;
            autoType_ = AutoSlideType.Close;
            isOpenDrag_ = false;
            grapImage.raycastTarget = true;
        }
    }

    /// <summary>
    /// 初期位置に戻す
    /// </summary>
    public void ResetPosition()
    {
        scrollRect.content.offsetMin = Vector2.zero;
        scrollRect.content.offsetMax = Vector2.zero;
        scrollRect.horizontalNormalizedPosition = GetCloseNormPos();
        prevNormPos_ = scrollRect.horizontalNormalizedPosition;
        clickBlocker.gameObject.SetActive(false);
        autoType_ = AutoSlideType.None;
        grapImage.raycastTarget = true;
    }

    /// <summary>
    /// クリックブロック範囲選択
    /// </summary>
    public void OnClickBlocker()
    {
        if (!isOpenDrag_ || !isOpenDragMove_)
        {
            Close();
        }
    }

    /// <summary>
    /// 開いたときのスクロール値
    /// </summary>
    /// <returns></returns>
    private float GetOpenNormPos()
    {
        if (isLeft) { return 0.0f; }
        else { return 1.0f; }
    }

    /// <summary>
    /// 閉じたときのスクロール値
    /// </summary>
    /// <returns></returns>
    private float GetCloseNormPos()
    {
        if (isLeft) { return 1.0f; }
        else { return 0.0f; }
    }

    /// <summary>
    /// イージング後の値を取得
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private float Easing(float t, float intarval)
    {
        if (intarval == 0.0f) { return 1.0f; }

        t /= intarval;
        t *= 2;
        if (t < 1)
        {
            return 0.5f * t * t * t;
        }
        else
        {
            t -= 2;
            return 0.5f * (t * t * t + 2);
        }
    }

    /// <summary>
    /// 画面のタップ判定
    /// </summary>
    /// <returns></returns>
    private bool IsScreenTouch(bool isMoved)
    {
        if (Application.isEditor)
        {
            // Editor操作
            if (isMoved) { return Input.GetMouseButton(0); }
            else { return Input.GetMouseButtonDown(0); }
        }
        else
        {
            // モバイル操作
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (isMoved) { return true; }
                else { return (touch.phase == TouchPhase.Began); }
            }
        }

        return false;
    }

    /// <summary>
    /// 画面のタップ座標を取得する
    /// </summary>
    /// <returns></returns>
    private Vector2 GetTouchPosition()
    {
        if (Application.isEditor)
        {
            // Editor操作
            return Input.mousePosition;
        }
        else
        {
            // モバイル操作
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                return touch.position;
            }
        }

        return Vector2.zero;
    }

    /// <summary>
    /// タッチを離す
    /// </summary>
    private void EndedTouch()
    {
        if (isLeft)
        {
            if (scrollRect.horizontalNormalizedPosition > 0.5f) { Close(); }
            else { Open(); }
        }
        else
        {
            if (scrollRect.horizontalNormalizedPosition < 0.5f) { Close(); }
            else { Open(); }
        }
    }
}
