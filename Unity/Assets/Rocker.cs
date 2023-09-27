using UnityEngine;
using UnityEngine.UI;

public class Rocker : ScrollRect
{
	protected float mRadius = 0f;

	protected override void Start()
	{
		base.Start();
		mRadius = (transform as RectTransform).sizeDelta.x * 0.5f;
	}

	public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnDrag(eventData);
		var contentPostion = this.content.anchoredPosition;
		if (contentPostion.magnitude > mRadius)
		{
			contentPostion = contentPostion.normalized * mRadius;
			SetContentAnchoredPosition(contentPostion);
		}
		Debug.Log(contentPostion.normalized);
	}
}
