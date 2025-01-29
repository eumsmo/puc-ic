using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Code found in: https://discussions.unity.com/t/rect-transform-size-limiter/730374/4
// Made some alterations to suport percentage

[ExecuteInEditMode]
public class RectSizeLimiter : UIBehaviour, ILayoutSelfController
{

    public enum UnitType { Pixels, Percentage }
    public struct MinMax {
        public float min, max;
        public MinMax(float min, float max) {
            this.min = min;
            this.max = max;
        }

        public Vector2 ToVector2() {
            return new Vector2(min, max);
        }
    }

    public RectTransform rectTransform;

    [SerializeField]
    protected Vector2 m_maxSize = Vector2.zero;

    [SerializeField]
    protected Vector2 m_minSize = Vector2.zero;


    private RectTransform _parentTransform;
    protected RectTransform parentTransform {
        get {
            if (_parentTransform != null) return _parentTransform;
            _parentTransform = GetComponentInParent<RectTransform>();
            return _parentTransform;
        }
    }

    public UnitType maxSizeType;
    public Vector2 maxSize
    {
        get { return m_maxSize; }
        set
        {
            if (m_maxSize != value)
            {
                m_maxSize = value;
                SetDirty();
            }
        }
    }


    public UnitType minSizeType;
    public Vector2 minSize
    {
        get { return m_minSize; }
        set
        {
            if (m_minSize != value)
            {
                m_minSize = value;
                SetDirty();
            }
        }
    }

    protected Vector2 maxSizeValue {
        get { 
            if (maxSizeType == UnitType.Pixels) return m_maxSize;
            else if (maxSizeType == UnitType.Percentage) return new Vector2(parentTransform.rect.width * m_maxSize.x, parentTransform.rect.height * m_maxSize.y);
            return new Vector2();
        }
    }

    protected Vector2 minSizeValue {
        get { 
            if (minSizeType == UnitType.Pixels) return m_minSize;
            else if (minSizeType == UnitType.Percentage) return new Vector2(parentTransform.rect.width * m_minSize.x, parentTransform.rect.height * m_minSize.y);
            return new Vector2();
        }
    }

    protected MinMax horizontalMinMax {
        get { 
            float min = (minSizeType == UnitType.Pixels) ? m_minSize.x : (parentTransform.rect.width * m_minSize.x);
            float max = (maxSizeType == UnitType.Pixels) ? m_maxSize.x : (parentTransform.rect.width * m_maxSize.x);

            return new MinMax(min, max);
        }
    }

    protected MinMax verticalMinMax {
        get { 
            float min = (minSizeType == UnitType.Pixels) ? m_minSize.y : (parentTransform.rect.height * m_minSize.y);
            float max = (maxSizeType == UnitType.Pixels) ? m_maxSize.y : (parentTransform.rect.height * m_maxSize.y);

            return new MinMax(min, max);
        }
    }

    private DrivenRectTransformTracker m_Tracker;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public void SetLayoutHorizontal()
    {
        MinMax horizontal = horizontalMinMax;

        if (horizontal.max > 0f && rectTransform.rect.width > horizontal.max)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontal.max);
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
        }

        if (horizontal.min > 0f && rectTransform.rect.width < horizontal.min)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontal.min);
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
        }

    }

    public void SetLayoutVertical()
    {
        MinMax vertical = verticalMinMax;
        
        if (vertical.max > 0f && rectTransform.rect.height > vertical.max)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vertical.max);
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
        }

        if (vertical.min > 0f && rectTransform.rect.height < vertical.min)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,vertical.min);
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
        }

    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        SetDirty();
    }
#endif

}