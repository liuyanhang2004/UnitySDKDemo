using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 拦截事件用的空Graphic
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class UIBlock : Graphic
{
    public override Texture mainTexture => null;
    public override bool raycastTarget => IsDestroyed() ? false : enabled;
    public override Material material => null;
    public override Material defaultMaterial => null;
    public override Material materialForRendering => null;

    private static List<Canvas> cacheList = new List<Canvas>();
    private Canvas cachedCanvas;

    protected override void OnEnable()
    {
        cacheCanvas();
        GraphicRegistry.RegisterGraphicForCanvas(cachedCanvas, this);
    }

    protected override void OnDisable()
    {
        GraphicRegistry.UnregisterGraphicForCanvas(cachedCanvas, this);
    }

    protected override void OnBeforeTransformParentChanged()
    {
        GraphicRegistry.UnregisterGraphicForCanvas(cachedCanvas, this);
    }

    protected override void OnTransformParentChanged()
    {
        cachedCanvas = null;

        if (!IsActive())
            return;

        cacheCanvas();
        GraphicRegistry.RegisterGraphicForCanvas(cachedCanvas, this);
    }

    protected override void OnCanvasHierarchyChanged()
    {
        // Use m_Cavas so we dont auto call CacheCanvas
        Canvas currentCanvas = cachedCanvas;

        // Clear the cached canvas. Will be fetched below if active.
        cachedCanvas = null;

        if (!IsActive())
            return;

        cacheCanvas();

        if (currentCanvas != cachedCanvas)
        {
            GraphicRegistry.UnregisterGraphicForCanvas(currentCanvas, this);

            // Only register if we are active and enabled as OnCanvasHierarchyChanged can get called
            // during object destruction and we dont want to register ourself and then become null.
            if (IsActive())
                GraphicRegistry.RegisterGraphicForCanvas(cachedCanvas, this);
        }
    }

    private void cacheCanvas()
    {
        gameObject.GetComponentsInParent(false, cacheList);
        if (cacheList.Count > 0)
        {
            // Find the first active and enabled canvas.
            for (int i = 0; i < cacheList.Count; ++i)
            {
                if (cacheList[i].isActiveAndEnabled)
                {
                    cachedCanvas = cacheList[i];
                    break;
                }
            }
        }
        else
        {
            cachedCanvas = null;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    protected override void UpdateGeometry()
    {
    }

    protected override void UpdateMaterial()
    {
    }

    protected override void OnRectTransformDimensionsChange()
    {
    }

    protected override void OnDidApplyAnimationProperties()
    {
    }

    public override void SetAllDirty()
    {
    }

    public override void SetLayoutDirty()
    {
    }

    public override void SetMaterialDirty()
    {
    }

    public override void SetVerticesDirty()
    {
    }

    public override void OnCullingChanged()
    {
    }

    public override void Rebuild(CanvasUpdate update)
    {
    }

    public override void LayoutComplete()
    {
    }

    public override void GraphicUpdateComplete()
    {
    }

    public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
    {
    }

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha,
        bool useRGB)
    {
    }

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
    {
    }

#if UNITY_EDITOR
    public override void OnRebuildRequested()
    {
    }

    protected override void OnValidate()
    {
    }
#endif
}