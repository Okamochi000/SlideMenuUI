using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FPS•\Ž¦
/// </summary>
public class FpsViewer : MonoBehaviour
{
    [SerializeField] [Range(0, 8)] private int decimalPoint = 2;
    [SerializeField] [Min(0.1f)] private float interval = 0.5f;

    private Text text_ = null;
    private int frameCount_ = 0;
    private float prevTime_ = 0.0f;
    private float fps_ = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        text_ = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string pointStr = "F" + decimalPoint;

        frameCount_++;
        float time = Time.realtimeSinceStartup - prevTime_;

        if (time >= interval)
        {
            fps_ = frameCount_ / time;
            text_.text = fps_.ToString(pointStr);
            frameCount_ = 0;
            prevTime_ = Time.realtimeSinceStartup;
        }
    }
}
