using UnityEngine;

public class AspectFitter : MonoBehaviour
{
    private Vector2 selfSize;
    private float selfScale;

    private void Awake()
    {
        selfSize = rect.sizeDelta;
        selfScale = selfSize.x / selfSize.y;
    }

    private void Start()
    {
        CalcSize();
    }

    public void CalcSize()
    {
        var scaleFactor = canvas.scaleFactor;

        var realWidth = Screen.width / scaleFactor;
        var realHeight = Screen.height / scaleFactor;

        // Debug.LogWarning($"RealWidth:{realWidth}, RealHeight:{realHeight}");

        if (realWidth > selfSize.x)
        {
            var newWidth = realWidth;
            var newHeight = newWidth / selfScale;
            SetSize(newWidth, newHeight);
        }
        else if (realHeight > selfSize.y)
        {
            var newHeight = realHeight;
            var newWidth = newHeight * selfScale;
            SetSize(newWidth, newHeight);
        }
    }

    private void SetSize(float newWidth, float newHeight)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }

    private RectTransform _rect;
    private RectTransform rect => _rect ? _rect : (_rect = (RectTransform) transform);
    private Canvas _canvas;
    private Canvas canvas => _canvas ? _canvas : (_canvas = GetComponentInParent<Canvas>());
}